using Sandbox;
using Editor;
using System;
using System.Collections.Generic;
using HalfEdgeMesh;

namespace StairsTool;

[Title("Stairs"), Category("Construction"), Icon("stairs")]
public class StairsTesting : Component 
{
    private float _width = 100.0f;
    private float _height = 200.0f;
    private float _depth = 300.0f;
    private int _stepCount = 10;
    private bool _doubleSided = true;
    private StairShape _stairShape = StairShape.Straight;
    private float _curveAngle = 90.0f;
    private float _landingHeight = 0.5f;
    private LandingDirection _landingDirection = LandingDirection.Right;
    private Material _material = null;
    private string _materialPath = "materials/dev/dev_gray.vmat";
    private int _materialRotation = 0;
    private bool _showMeshCollider = false;
	private bool _alwaysCenterMesh = true;

    [Property] 
    public float Width 
    { 
        get => _width;
        set
        {
            if (_width == value) return;
            _width = value;
            RegenerateMesh();
        }
    }

    [Property] 
    public float Height 
    { 
        get => _height;
        set
        {
            if (_height == value) return;
            _height = value;
            RegenerateMesh();
        }
    }

    [Property] 
    public float Depth 
    { 
        get => _depth;
        set
        {
            if (_depth == value) return;
            _depth = value;
            RegenerateMesh();
        }
    }

    [Property, Range(2, 64)] 
    public int StepCount 
    { 
        get => _stepCount;
        set
        {
            if (_stepCount == value) return;
            _stepCount = value;
            RegenerateMesh();
        }
    }


	//TODO: split the vertices into top/side&bottom to allow for only top/all sides option to save on polycount
    //[Property]
    public bool DoubleSided = true;
    /*{
        get => _doubleSided;
        set
        {
            if (_doubleSided == value) return;
            _doubleSided = value;
            RegenerateMesh();
        }
    }*/
    
    [Property]
    public StairShape ShapeType
    {
        get => _stairShape;
        set
        {
            if (_stairShape == value) return;
            _stairShape = value;
            RegenerateMesh();
        }
    }
    
	[ShowIf("ShapeType",StairShape.Curved)]
    [Property, Range(1, 360)]
    public float CurveAngle
    {
        get => _curveAngle;
        set
        {
            if (_curveAngle == value) return;
            _curveAngle = value;
            if (_stairShape == StairShape.Curved)
                RegenerateMesh();
        }
    }
    
	[ShowIf("ShapeType",StairShape.Landing)]
    [Property, Range(0.1f, 0.9f)]
    public float LandingHeightRatio
    {
        get => _landingHeight;
        set
        {
            if (_landingHeight == value) return;
            _landingHeight = Math.Clamp(value, 0.1f, 0.9f);
            if (_stairShape == StairShape.Landing)
                RegenerateMesh();
        }
    }

    [ShowIf("ShapeType",StairShape.Landing)]
    [Property]
    public LandingDirection LandingDir
    {
        get => _landingDirection;
        set
        {
            if (_landingDirection == value) return;
            _landingDirection = value;
            if (_stairShape == StairShape.Landing)
                RegenerateMesh();
        }
    }

    [Property, ResourceType("material")]
    public Material Material
    {
        get => _material;
        set
        {
            if (_material == value) return;
            _material = value;
            RegenerateMesh();
        }
    }
    //TODO: Add a property to set the material rotation from the editor
    //[Property, Range(0, 360), Title("Material Rotation"), Description("Rotation angle for textures on stair faces in degrees")]
    public float MaterialRotation
    {
        get => _materialRotation;
        set
        {
            if (_materialRotation == value) return;
            _materialRotation = (int)value;
            RegenerateMesh();
        }
    }

    [Property, Description("Show or hide the mesh collider in the editor"), Category("Advanced")] 
    public bool ShowMeshCollider 
    { 
        get => _showMeshCollider;
        set
        {
            if (_showMeshCollider == value) return;
            _showMeshCollider = value;
            UpdateMeshColliderVisibility();
        }
    }

	[Property, Description("Show or hide the mesh collider in the editor"), Category("Advanced")] 
    public bool AlwaysCenterMesh 
    { 
        get => _alwaysCenterMesh;
		set
		{
			if (_alwaysCenterMesh == value) return;
			_alwaysCenterMesh = value;
			UpdateMeshColliderVisibility();
		}
    }

    private PolygonMesh _mesh;
    
    // Get the mesh component, creating it if it doesn't exist
    private MeshComponent MeshComponent => GameObject?.Components.GetOrCreate<MeshComponent>();
    
    protected override void OnAwake()
    {
        base.OnAwake();
        
        RegenerateMesh();
    }
      protected override void OnEnabled()
    {
        base.OnEnabled();
        RegenerateMesh();
        UpdateMeshColliderVisibility();
    }

    private void RegenerateMesh()
    {
        if (!GameObject.IsValid)
            return;
        
        try
        {
            _mesh = new PolygonMesh();
            
            switch (_stairShape)
            {
                case StairShape.Straight:
                    GenerateStraightStairMesh(_mesh);
                    break;
                case StairShape.Curved:
                    GenerateCurvedStairMesh(_mesh);
                    break;
                case StairShape.Landing:
                    GenerateLandingStairMesh(_mesh);
                    break;
                default:
                    GenerateStraightStairMesh(_mesh);
                    break;
            }
            
            Material material = _material;
            if (material == null)
            {
                Log.Warning($"Material is null, trying default material...");
                material = Material.Load("materials/default.vmat");
                
                if (material == null)
                {
                    Log.Error("Failed to load materials");
                    return;
                }
            }
            
            foreach (var face in _mesh.FaceHandles)
            {
                _mesh.SetFaceMaterial(face, material);
                _mesh.TextureAlignToGrid(new Transform().WithPosition(Vector3.Zero).WithRotation(Rotation.FromRoll(_materialRotation)));
            }
			if (_alwaysCenterMesh){
				CenterMesh();
			}
			var meshComponent = MeshComponent;
            if (meshComponent != null)
            {
                meshComponent.Mesh = _mesh;
                Log.Info("Stair mesh generated successfully");
                
                // Update the mesh collider visibility after generating the mesh
                UpdateMeshColliderVisibility();
            }
            else
            {
                Log.Error("Failed to create MeshComponent");
            }

			
        }
        catch (Exception ex)
        {
            Log.Error($"Exception in RegenerateMesh: {ex.Message}");
            Log.Error(ex.StackTrace);
        }
    }

    [Property, Button("Center Mesh"), Description("Centers the mesh at the origin"), Category("Advanced")]
    public void CenterMesh()
    {
        if (_mesh == null || !GameObject.IsValid)
            return;
        
        try
        {
            // Calculate the bounds of the mesh
            BBox bounds = _mesh.CalculateBounds();
            
            // Calculate the center offset
            Vector3 centerOffset = bounds.Center;
            
            // Apply this offset to all vertices
            foreach (var vertexHandle in _mesh.VertexHandles)
            {
                Vector3 currentPos = _mesh.GetVertexPosition(vertexHandle);
                _mesh.SetVertexPosition(vertexHandle, currentPos - centerOffset);
            }
            
            // Update the mesh component
            MeshComponent.Mesh = _mesh;
            
            Log.Info("Mesh successfully centered at origin");
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to center mesh: {ex.Message}");
            Log.Error(ex.StackTrace);
        }
    }

    private void GenerateStraightStairMesh(PolygonMesh mesh)
    {
        if (mesh == null) return;

        float stepHeight = Height / StepCount;
        float stepDepth = Depth / StepCount;
        
        // Create vertices for the full staircase outline for enclosing panels if needed
        var leftSideVertices = new List<VertexHandle>();
        var rightSideVertices = new List<VertexHandle>();
        var bottomVertices = new List<VertexHandle>();
        var topVertices = new List<VertexHandle>();
        var backVertices = new List<VertexHandle>();
        
        // Store corner vertices for the complete staircase
        VertexHandle backBottomLeft = default;
        VertexHandle backBottomRight = default;
        VertexHandle backTopLeft = default;
        VertexHandle backTopRight = default;
        
        // Generate each step individually, making sure every step is fully enclosed
        for (var i = 0; i < StepCount; i++)
        {
            float z = stepHeight * i;
            float x = stepDepth * i;
            float nextZ = stepHeight * (i + 1);
            float nextX = stepDepth * (i + 1);

            // Add vertices for this step
            var v1 = mesh.AddVertex(new Vector3(x, -Width/2, z));         // Bottom left front
            var v2 = mesh.AddVertex(new Vector3(x, Width/2, z));          // Bottom right front
            var v3 = mesh.AddVertex(new Vector3(x, -Width/2, nextZ));     // Top left front
            var v4 = mesh.AddVertex(new Vector3(x, Width/2, nextZ));      // Top right front
            var v5 = mesh.AddVertex(new Vector3(nextX, -Width/2, nextZ)); // Top left back
            var v6 = mesh.AddVertex(new Vector3(nextX, Width/2, nextZ));  // Top right back
            var v7 = mesh.AddVertex(new Vector3(nextX, -Width/2, z));     // Bottom left back
            var v8 = mesh.AddVertex(new Vector3(nextX, Width/2, z));      // Bottom right back

            // Store references to full staircase bounds if needed for outer panels
            if (i == 0)
            {
                bottomVertices.Add(v1);
                bottomVertices.Add(v2);
                bottomVertices.Add(v8);
                bottomVertices.Add(v7);
                
                leftSideVertices.Add(v1);
                leftSideVertices.Add(v3);
                
                rightSideVertices.Add(v2);
                rightSideVertices.Add(v4);
                
                // Store back bottom corners
                backBottomLeft = v7;
                backBottomRight = v8;
            }
            
            if (i == StepCount - 1)
            {
                // Add top vertices for the top enclosing face
                topVertices.Add(v3);
                topVertices.Add(v5);
                topVertices.Add(v6);
                topVertices.Add(v4);
                
                // Store back top corners
                backTopLeft = v5;
                backTopRight = v6;
                
                leftSideVertices.Add(v5);
                leftSideVertices.Add(v7);
                
                rightSideVertices.Add(v6);
                rightSideVertices.Add(v8);
            }
            else if (i == 0)
            {
                leftSideVertices.Add(v3);
                rightSideVertices.Add(v4);
            }

            // Each step gets ALL faces with correct winding order to face outward
            
            // 1. Front face (vertical part of step) - facing front
            mesh.AddFace(v1, v3, v4, v2);
            
            // 2. Top face (horizontal part of step) - facing up
            mesh.AddFace(v3, v5, v6, v4);
            
            // 3. Back face of step - facing back
            mesh.AddFace(v5, v7, v8, v6);
            
            // 4. Left side face of step - facing left
            mesh.AddFace(v1, v3, v5, v7);
            
            // 5. Right side face of step - facing right
            mesh.AddFace(v2, v4, v6, v8);
            
            // 6. Bottom face of step - facing down
            mesh.AddFace(v1, v2, v8, v7);
            
            // If double sided is enabled, add reverse winding faces for inside visibility
            if (_doubleSided)
            {
                // 1. Front face (reverse)
                mesh.AddFace(v2, v4, v3, v1);
                
                // 2. Top face (reverse)
                mesh.AddFace(v4, v6, v5, v3);
                
                // 3. Back face (reverse)
                mesh.AddFace(v6, v8, v7, v5);
                
                // 4. Left side face (reverse)
                mesh.AddFace(v7, v5, v3, v1);
                
                // 5. Right side face (reverse)
                mesh.AddFace(v8, v6, v4, v2);
                
                // 6. Bottom face (reverse)
                mesh.AddFace(v7, v8, v2, v1);
            }
        }
        
        // If double sided is enabled, also add the full enclosure panels with correct winding
        if (_doubleSided)
        {
            // Full left side panel with correct winding to face outward
            if (leftSideVertices.Count >= 4)
            {
                var leftPanelVertices = new List<VertexHandle>();
                // Reverse the left side vertices to make them face outward
                for (int i = leftSideVertices.Count - 1; i >= 0; i--)
                {
                    leftPanelVertices.Add(leftSideVertices[i]);
                }
                
                // Add full left side panel face (facing left)
                mesh.AddFace(leftPanelVertices.ToArray());
                
                // Add reverse-wound face for double-sided rendering
                mesh.AddFace(leftSideVertices.ToArray());
            }
            
            // Full right side panel with correct winding to face outward
            if (rightSideVertices.Count >= 4)
            {
                // Add right side panel face (facing right)
                mesh.AddFace(rightSideVertices.ToArray());
                
                // Add reverse-wound face for double-sided rendering
                var rightPanelVertices = new List<VertexHandle>();
                for (int i = rightSideVertices.Count - 1; i >= 0; i--)
                {
                    rightPanelVertices.Add(rightSideVertices[i]);
                }
                mesh.AddFace(rightPanelVertices.ToArray());
            }
            
            // Add top enclosing face with correct winding to face upward
            if (topVertices.Count > 0)
            {
                // Add top face facing upward
                mesh.AddFace(topVertices.ToArray());
                
                // Add reverse-wound face for double-sided rendering
                var topVerticesReversed = new List<VertexHandle>(topVertices);
                topVerticesReversed.Reverse();
                mesh.AddFace(topVerticesReversed.ToArray());
            }
            
            // Add back panel with correct winding to face backward
            if (backBottomLeft.IsValid && backBottomRight.IsValid && backTopRight.IsValid && backTopLeft.IsValid)
            {
                // Add back face with correct winding to face backward
                backVertices.Add(backBottomLeft);
                backVertices.Add(backTopLeft);
                backVertices.Add(backTopRight);
                backVertices.Add(backBottomRight);
                
                mesh.AddFace(backVertices.ToArray());
                
                // Add reverse-wound face for double-sided rendering
                var backVerticesReversed = new List<VertexHandle>(backVertices);
                backVerticesReversed.Reverse();
                mesh.AddFace(backVerticesReversed.ToArray());
            }
        }

        // Apply texture alignment to all faces
        //mesh.TextureAlignToGrid(Transform.Zero);
    }
    
    private void GenerateCurvedStairMesh(PolygonMesh mesh)
    {
        if (mesh == null) return;
        
        float stepHeight = Height / StepCount;
        float totalAngle = MathF.PI * _curveAngle / 180.0f; // Convert degrees to radians
        float anglePerStep = totalAngle / StepCount;
        
        // Calculate the radius based on the desired depth (outer perimeter)
        float outerRadius = Depth / totalAngle;
        float innerRadius = outerRadius - Width;
        
        // Create vertices for the full staircase outline
        var innerSideVertices = new List<VertexHandle>();
        var outerSideVertices = new List<VertexHandle>();
        var bottomVertices = new List<VertexHandle>();
        var topVertices = new List<VertexHandle>();
        
        // Store the first and last steps' vertices for enclosing the sides
        VertexHandle[] firstStepVertices = new VertexHandle[4];
        VertexHandle[] lastStepVertices = new VertexHandle[4];
        
        // Generate each step individually
        for (var i = 0; i < StepCount; i++)
        {
            float z = stepHeight * i;
            float nextZ = stepHeight * (i + 1);
            
            float angle = anglePerStep * i;
            float nextAngle = anglePerStep * (i + 1);
            
            // Calculate inner and outer points at current and next angle
            float sinAngle = MathF.Sin(angle);
            float cosAngle = MathF.Cos(angle);
            float sinNextAngle = MathF.Sin(nextAngle);
            float cosNextAngle = MathF.Cos(nextAngle);
            
            Vector3 innerPoint = new Vector3(innerRadius * sinAngle, innerRadius * cosAngle, z);
            Vector3 outerPoint = new Vector3(outerRadius * sinAngle, outerRadius * cosAngle, z);
            Vector3 innerNextPoint = new Vector3(innerRadius * sinNextAngle, innerRadius * cosNextAngle, z);
            Vector3 outerNextPoint = new Vector3(outerRadius * sinNextAngle, outerRadius * cosNextAngle, z);
            
            Vector3 innerPointTop = new Vector3(innerRadius * sinAngle, innerRadius * cosAngle, nextZ);
            Vector3 outerPointTop = new Vector3(outerRadius * sinAngle, outerRadius * cosAngle, nextZ);
            Vector3 innerNextPointTop = new Vector3(innerRadius * sinNextAngle, innerRadius * cosNextAngle, nextZ);
            Vector3 outerNextPointTop = new Vector3(outerRadius * sinNextAngle, outerRadius * cosNextAngle, nextZ);
            
            // Add vertices for this step
            var v1 = mesh.AddVertex(innerPoint);          // Bottom inner
            var v2 = mesh.AddVertex(outerPoint);          // Bottom outer
            var v3 = mesh.AddVertex(innerPointTop);       // Top inner
            var v4 = mesh.AddVertex(outerPointTop);       // Top outer
            var v5 = mesh.AddVertex(innerNextPointTop);   // Top next inner
            var v6 = mesh.AddVertex(outerNextPointTop);   // Top next outer
            var v7 = mesh.AddVertex(innerNextPoint);      // Bottom next inner
            var v8 = mesh.AddVertex(outerNextPoint);      // Bottom next outer
            
            // Store first and last steps' vertices for side enclosure
            if (i == 0)
            {
                firstStepVertices[0] = v1; // Bottom inner
                firstStepVertices[1] = v2; // Bottom outer
                firstStepVertices[2] = v3; // Top inner
                firstStepVertices[3] = v4; // Top outer
                
                // Add to side vertices lists
                innerSideVertices.Add(v1);
                innerSideVertices.Add(v3);
                
                outerSideVertices.Add(v2);
                outerSideVertices.Add(v4);
                
                // Add to bottom vertices
                bottomVertices.Add(v1);
                bottomVertices.Add(v2);
                bottomVertices.Add(v8);
                bottomVertices.Add(v7);
            }
            
            if (i == StepCount - 1)
            {
                lastStepVertices[0] = v5; // Top next inner
                lastStepVertices[1] = v6; // Top next outer
                lastStepVertices[2] = v7; // Bottom next inner
                lastStepVertices[3] = v8; // Bottom next outer
                
                // Add to side vertices lists
                innerSideVertices.Add(v5);
                innerSideVertices.Add(v7);
                
                outerSideVertices.Add(v6);
                outerSideVertices.Add(v8);
                
                // Add top vertices for the top enclosing face
                topVertices.Add(v3);
                topVertices.Add(v5);
                topVertices.Add(v6);
                topVertices.Add(v4);
            }
            
            // Front vertical face of step
            mesh.AddFace(v1, v3, v4, v2);
            
            // Top horizontal face of step
            mesh.AddFace(v3, v5, v6, v4);
            
            // Back vertical face of step
            mesh.AddFace(v5, v7, v8, v6);
            
            // Inner side face of step
            mesh.AddFace(v1, v3, v5, v7);
            
            // Outer side face of step
            mesh.AddFace(v2, v4, v6, v8);
            
            // Bottom face of step
            mesh.AddFace(v1, v2, v8, v7);
            
            // If double sided is enabled, add reverse winding faces
            if (_doubleSided)
            {
                // Front face (reverse)
                mesh.AddFace(v2, v4, v3, v1);
                
                // Top face (reverse)
                mesh.AddFace(v4, v6, v5, v3);
                
                // Back face (reverse)
                mesh.AddFace(v6, v8, v7, v5);
                
                // Inner side face (reverse)
                mesh.AddFace(v7, v5, v3, v1);
                
                // Outer side face (reverse)
                mesh.AddFace(v8, v6, v4, v2);
                
                // Bottom face (reverse)
                mesh.AddFace(v7, v8, v2, v1);
            }
        }
        
        // Add enclosing faces for start and end of staircase if double sided
        if (_doubleSided && firstStepVertices[0].IsValid && lastStepVertices[0].IsValid)
        {
            // Start face (facing toward start of stairs)
            var startFace = new List<VertexHandle>
            {
                firstStepVertices[0], // Bottom inner
                firstStepVertices[2], // Top inner
                firstStepVertices[3], // Top outer
                firstStepVertices[1]  // Bottom outer
            };
            mesh.AddFace(startFace.ToArray());
            
            // Reverse winding for double-sided rendering
            var startFaceReversed = new List<VertexHandle>(startFace);
            startFaceReversed.Reverse();
            mesh.AddFace(startFaceReversed.ToArray());
            
            // End face (facing toward end of stairs)
            var endFace = new List<VertexHandle>
            {
                lastStepVertices[2], // Bottom next inner
                lastStepVertices[0], // Top next inner
                lastStepVertices[1], // Top next outer
                lastStepVertices[3]  // Bottom next outer
            };
            mesh.AddFace(endFace.ToArray());
            
            // Reverse winding for double-sided rendering
            var endFaceReversed = new List<VertexHandle>(endFace);
            endFaceReversed.Reverse();
            mesh.AddFace(endFaceReversed.ToArray());
            
            // Add top enclosing face
            if (topVertices.Count > 0)
            {
                mesh.AddFace(topVertices.ToArray());
                
                var topVerticesReversed = new List<VertexHandle>(topVertices);
                topVerticesReversed.Reverse();
                mesh.AddFace(topVerticesReversed.ToArray());
            }
        }
        
        // Apply texture alignment to all faces
        //mesh.TextureAlignToGrid(Transform.Zero);
    }
    
    private void GenerateLandingStairMesh(PolygonMesh mesh)
    {
        if (mesh == null) return;
        
        // Calculate the landing height position (relative to total height)
        float landingHeightPos = Height * _landingHeight;
        
        // Calculate step counts and dimensions for each flight
        int firstFlightSteps = (int)(StepCount * _landingHeight);
        int secondFlightSteps = StepCount - firstFlightSteps;
        
        // Ensure we have at least one step in each flight
        if (firstFlightSteps < 1) firstFlightSteps = 1;
        if (secondFlightSteps < 1) secondFlightSteps = 1;
        
        float firstFlightHeight = landingHeightPos;
        float secondFlightHeight = Height - landingHeightPos;
        
        float firstStepHeight = firstFlightHeight / firstFlightSteps;
        float secondStepHeight = secondFlightHeight / secondFlightSteps;
        
        // First flight uses half the depth for straight and U shape
        float firstFlightDepth = Depth / 2;
        // Second flight uses half the depth for straight and U shape
        float secondFlightDepth = Depth / 2;
        
        float firstStepDepth = firstFlightDepth / firstFlightSteps;
        float secondStepDepth = secondFlightDepth / secondFlightSteps;
        
        // Landing dimensions
        // For U-shape, we need to adjust the landing depth
        float landingDepth = /*_landingDirection == LandingDirection.U ? Width :*/ Width;
        
        // Generate the first flight of stairs (going up)
        GenerateStairFlight(
            mesh, 
            0, 
            0, 
            firstFlightSteps, 
            firstStepHeight, 
            firstStepDepth, 
            Width, 
            false);
        
        // Generate the landing platform
        // Use landing top height (landing height position + step height) for the second flight starting position
        float landingTopHeight = landingHeightPos + firstStepHeight;
        
        // Create landing connection points based on direction
        var landingTopFrontLeft = new Vector3(0, 0, 0);
        var landingTopFrontRight = new Vector3(0, 0, 0);
        
        switch (_landingDirection)
        {
            case LandingDirection.Straight:
                // For straight, connection points are at the back of the landing
                landingTopFrontLeft = new Vector3(firstFlightDepth + landingDepth, -Width / 2, landingTopHeight);
                landingTopFrontRight = new Vector3(firstFlightDepth + landingDepth, Width / 2, landingTopHeight);
                break;
                
            case LandingDirection.Left:
                // For left turn, connection points are at the left side of the landing
                landingTopFrontLeft = new Vector3(firstFlightDepth, Width / 2, landingTopHeight);
                landingTopFrontRight = new Vector3(firstFlightDepth + Width, Width / 2, landingTopHeight);
                break;
                
            case LandingDirection.Right:
                // For right turn, connection points are at the right side of the landing
                landingTopFrontLeft = new Vector3(firstFlightDepth, -Width / 2, landingTopHeight);
                landingTopFrontRight = new Vector3(firstFlightDepth + Width, -Width / 2, landingTopHeight);
                break;
                
            /*case LandingDirection.U:
                // For U shape, connection points are at the front of the landing (same side as first flight)
                landingTopFrontLeft = new Vector3(0, -Width / 2, landingTopHeight);
                landingTopFrontRight = new Vector3(0, Width / 2, landingTopHeight);
                break;*/
        }

        GenerateLandingPlatform(
            mesh, 
            firstFlightDepth,  // x position at end of first flight
            landingHeightPos,  // z position at landing height
            landingDepth,     // depth of landing platform
            Width,            // width of landing platform
            firstStepHeight); // height of a step (for consistency)
        
        // Position the second flight based on the landing direction
        // Adjusted to be from perspective of someone at the bottom looking up
        switch (_landingDirection)
        {
            case LandingDirection.Straight:
                // For straight, continue in the same direction (along X axis)
                GenerateStairFlight(
                    mesh,
                    firstFlightDepth + landingDepth, // Start at the edge of the landing
                    landingTopHeight, // Start at the top of landing platform
                    secondFlightSteps,
                    secondStepHeight,
                    secondStepDepth,
                    Width,
                    false,
                    false,
                    landingTopFrontLeft,
                    landingTopFrontRight); // Connect to landing front vertices
                break;
                
            case LandingDirection.Left:
                // For left turn, place stairs on the left side (-Y) of the landing
                // From perspective of someone at bottom looking up
                
                GenerateStairFlight(
                    mesh,
                    firstFlightDepth, // Start at the beginning of the landing
                    landingTopHeight, // Start at the top of landing platform
                    secondFlightSteps,
                    secondStepHeight,
                    secondStepDepth,
                    Width,
                    true,  // true = perpendicular to first flight (along Y axis)
                    false,
                    landingTopFrontLeft,
                    landingTopFrontRight); // Connect to landing front vertices
                break;
                
            case LandingDirection.Right:
                // For right turn, place stairs on the right side (+Y) of the landing
                // From perspective of someone at bottom looking up
                GenerateStairFlight(
                    mesh,
                    firstFlightDepth, // Start at the beginning of the landing
                    landingTopHeight, // Start at the top of landing platform
                    secondFlightSteps,
                    secondStepHeight,
                    secondStepDepth,
                    Width,
                    true,  // true = perpendicular to first flight (along Y axis)
                    true,
                    landingTopFrontLeft,
                    landingTopFrontRight); // Connect to landing front vertices
                break;
                
            /*case LandingDirection.U:
                // For U-shaped stairs, place the second flight going back in the opposite direction
                // from the first flight (back along negative X axis)
                GenerateStairFlight(
                    mesh,
                    0, // Start at x=0 (same side as first flight)
                    landingTopHeight, // Start at the top of landing platform
                    secondFlightSteps,
                    secondStepHeight,
                    secondStepDepth,
                    Width,
                    false, // false = same axis as first flight (along X axis)
                    true,  // true = opposite direction as first flight
                    landingTopFrontLeft,
                    landingTopFrontRight); // Connect to landing front vertices
                break;*/
        }
    }
    
    private void GenerateStairFlight(PolygonMesh mesh, float startX, float startZ, int steps, float stepHeight, float stepDepth, float width, bool perpendicular, bool negativeDirection = false, Vector3? landingConnectLeft = null, Vector3? landingConnectRight = null)
    {
        // Lists to store vertices for enclosing panels
        var leftSideVertices = new List<VertexHandle>();
        var rightSideVertices = new List<VertexHandle>();
        var bottomFaceVertices = new List<VertexHandle>();
        var topFaceVertices = new List<VertexHandle>();
        var backFaceVertices = new List<VertexHandle>();
        
        for (var i = 0; i < steps; i++)
        {
            float z = startZ + (stepHeight * i);
            float nextZ = startZ + (stepHeight * (i + 1));
            
            Vector3 v1, v2, v3, v4, v5, v6, v7, v8;
            
            if (!perpendicular)
            {
                // Standard direction (along X axis)
                float x = startX + (stepDepth * i);
                float nextX = startX + (stepDepth * (i + 1));
                
                v1 = new Vector3(x, -width/2, z);         // Bottom left front
                v2 = new Vector3(x, width/2, z);          // Bottom right front
                v3 = new Vector3(x, -width/2, nextZ);     // Top left front
                v4 = new Vector3(x, width/2, nextZ);      // Top right front
                v5 = new Vector3(nextX, -width/2, nextZ); // Top left back
                v6 = new Vector3(nextX, width/2, nextZ);  // Top right back
                v7 = new Vector3(nextX, -Width/2, z);     // Bottom left back
                v8 = new Vector3(nextX, Width/2, z);      // Bottom right back
            }
            else
            {
                // Perpendicular direction (along Y axis)
                float direction = negativeDirection ? -1 : 1; // Determine if we're going in positive or negative Y direction
                
                if (negativeDirection)
                {
                    // Going in negative Y direction (RIGHT turn)
                    float y = -width/2 - (stepDepth * i);
                    float nextY = -width/2 - (stepDepth * (i + 1));
                    
                    v1 = new Vector3(startX, y, z);           // Bottom left front
                    v2 = new Vector3(startX + width, y, z);   // Bottom right front
                    v3 = new Vector3(startX, y, nextZ);       // Top left front
                    v4 = new Vector3(startX + width, y, nextZ);// Top right front
                    v5 = new Vector3(startX, nextY, nextZ);   // Top left back
                    v6 = new Vector3(startX + width, nextY, nextZ); // Top right back
                    v7 = new Vector3(startX, nextY, z);       // Bottom left back
                    v8 = new Vector3(startX + width, nextY, z); // Bottom right back
                }
                else
                {
                    // Going in positive Y direction (LEFT turn)
                    float y = width/2 + (stepDepth * i);
                    float nextY = width/2 + (stepDepth * (i + 1));
                    
                    v1 = new Vector3(startX, y, z);           // Bottom left front
                    v2 = new Vector3(startX + width, y, z);   // Bottom right front
                    v3 = new Vector3(startX, y, nextZ);       // Top left front
                    v4 = new Vector3(startX + width, y, nextZ);// Top right front
                    v5 = new Vector3(startX, nextY, nextZ);   // Top left back
                    v6 = new Vector3(startX + width, nextY, nextZ); // Top right back
                    v7 = new Vector3(startX, nextY, z);       // Bottom left back
                    v8 = new Vector3(startX + width, nextY, z); // Bottom right back
                }
            }
            
            // For the first step of the second flight, connect to the landing if landing vertices are provided
            if (i == 0 && landingConnectLeft.HasValue && landingConnectRight.HasValue)
            {
                // Use the landing's vertices for the front bottom points of the first step
                v1 = landingConnectLeft.Value;  // Bottom left front (connects to landing)
                v2 = landingConnectRight.Value; // Bottom right front (connects to landing)
            }
            
            var vh1 = mesh.AddVertex(v1);
            var vh2 = mesh.AddVertex(v2);
            var vh3 = mesh.AddVertex(v3);
            var vh4 = mesh.AddVertex(v4);
            var vh5 = mesh.AddVertex(v5);
            var vh6 = mesh.AddVertex(v6);
            var vh7 = mesh.AddVertex(v7);
            var vh8 = mesh.AddVertex(v8);
            
            // Store references for enclosure panels
            if (i == 0)
            {
                leftSideVertices.Add(vh1);
                leftSideVertices.Add(vh3);
                
                rightSideVertices.Add(vh2);
                rightSideVertices.Add(vh4);
                
                bottomFaceVertices.Add(vh1);
                bottomFaceVertices.Add(vh2);
                bottomFaceVertices.Add(vh8);
                bottomFaceVertices.Add(vh7);
            }
            
            if (i == steps - 1)
            {
                // Add vertices for top face of last step
                topFaceVertices.Add(vh3);
                topFaceVertices.Add(vh4);
                topFaceVertices.Add(vh6);
                topFaceVertices.Add(vh5);
                
                // Add back vertices
                backFaceVertices.Add(vh5);
                backFaceVertices.Add(vh6);
                backFaceVertices.Add(vh8);
                backFaceVertices.Add(vh7);
                
                // Complete left side vertices
                leftSideVertices.Add(vh5);
                leftSideVertices.Add(vh7);
                
                // Complete right side vertices
                rightSideVertices.Add(vh6);
                rightSideVertices.Add(vh8);
            }
            
            if (perpendicular && !negativeDirection)
            {
                // For LEFT landing direction, we need to flip some of the face windings
                // because the stairs are going in the positive Y axis direction
                
                // Front face (vertical part of step) - facing front
                mesh.AddFace(vh2, vh4, vh3, vh1);
                
                // Top face (horizontal part of step) - facing up
                mesh.AddFace(vh4, vh6, vh5, vh3);
                
                // Back face of step - facing back (away from first flight)
                mesh.AddFace(vh6, vh8, vh7, vh5);
                
                // Left side face of step (which is actually the outer side in this orientation)
                mesh.AddFace(vh2, vh4, vh6, vh8);
                
                // Right side face of step (which is actually the inner side in this orientation)
                mesh.AddFace(vh7, vh5, vh3, vh1);
                
                // Bottom face of step - facing down
                mesh.AddFace(vh7, vh8, vh2, vh1);
                
                // If double sided is enabled, add reverse winding faces for inside visibility
                if (_doubleSided)
                {
                    // 1. Front face (reverse)
                    mesh.AddFace(vh1, vh3, vh4, vh2);
                    
                    // 2. Top face (reverse)
                    mesh.AddFace(vh3, vh5, vh6, vh4);
                    
                    // 3. Back face (reverse)
                    mesh.AddFace(vh5, vh7, vh8, vh6);
                    
                    // 4. Left side face (reverse) (outer side)
                    mesh.AddFace(vh8, vh6, vh4, vh2);
                    
                    // 5. Right side face (reverse) (inner side)
                    mesh.AddFace(vh1, vh3, vh5, vh7);
                    
                    // 6. Bottom face (reverse)
                    mesh.AddFace(vh1, vh2, vh8, vh7);
                }
            }
            else
            {
                // Standard winding order for both straight stairs and RIGHT landing direction
                
                // Front face (vertical part of step) - facing front
                mesh.AddFace(vh1, vh3, vh4, vh2);
                
                // Top face (horizontal part of step) - facing up
                mesh.AddFace(vh3, vh5, vh6, vh4);
                
                // Back face of step - facing back
                mesh.AddFace(vh5, vh7, vh8, vh6);
                
                // Left side face of step - facing left
                mesh.AddFace(vh1, vh3, vh5, vh7);
                
                // Right side face of step - facing right
                mesh.AddFace(vh2, vh4, vh6, vh8);
                
                // Bottom face of step - facing down
                mesh.AddFace(vh1, vh2, vh8, vh7);
                
                // If double sided is enabled, add reverse winding faces for inside visibility
                if (_doubleSided)
                {
                    // 1. Front face (reverse)
                    mesh.AddFace(vh2, vh4, vh3, vh1);
                    
                    // 2. Top face (reverse)
                    mesh.AddFace(vh4, vh6, vh5, vh3);
                    
                    // 3. Back face (reverse)
                    mesh.AddFace(vh6, vh8, vh7, vh5);
                    
                    // 4. Left side face (reverse)
                    mesh.AddFace(vh7, vh5, vh3, vh1);
                    
                    // 5. Right side face (reverse)
                    mesh.AddFace(vh8, vh6, vh4, vh2);
                    
                    // 6. Bottom face (reverse)
                    mesh.AddFace(vh7, vh8, vh2, vh1);
                }
            }
        }
    }
    
    private void GenerateLandingPlatform(PolygonMesh mesh, float x, float z, float depth, float width, float height)
    {
        // Create a simple cube for the landing platform
        // Define the 8 vertices of the landing platform
        var v1 = new Vector3(x, -width/2, z);             // Bottom left front
        var v2 = new Vector3(x, width/2, z);              // Bottom right front
        var v3 = new Vector3(x, -width/2, z + height);    // Top left front
        var v4 = new Vector3(x, width/2, z + height);     // Top right front
        var v5 = new Vector3(x + depth, -width/2, z + height); // Top left back
        var v6 = new Vector3(x + depth, width/2, z + height);  // Top right back
        var v7 = new Vector3(x + depth, -width/2, z);     // Bottom left back
        var v8 = new Vector3(x + depth, width/2, z);      // Bottom right back
        
        // Add vertices to the mesh
        var vh1 = mesh.AddVertex(v1);
        var vh2 = mesh.AddVertex(v2);
        var vh3 = mesh.AddVertex(v3);
        var vh4 = mesh.AddVertex(v4);
        var vh5 = mesh.AddVertex(v5);
        var vh6 = mesh.AddVertex(v6);
        var vh7 = mesh.AddVertex(v7);
        var vh8 = mesh.AddVertex(v8);
        
        // Add faces with correct winding order to face outward
        // Front face
        mesh.AddFace(vh1, vh3, vh4, vh2);
        
        // Top face
        mesh.AddFace(vh3, vh5, vh6, vh4);
        
        // Back face
        mesh.AddFace(vh5, vh7, vh8, vh6);
        
        // Left side face
        mesh.AddFace(vh1, vh7, vh5, vh3);
        
        // Right side face
        mesh.AddFace(vh2, vh4, vh6, vh8);
        
        // Bottom face
        mesh.AddFace(vh1, vh2, vh8, vh7);
        
        // Always add reverse winding faces for the landing platform to avoid see-through issues
        // This ensures you won't see through the platform from any angle
        
        // Front face (reverse)
        mesh.AddFace(vh2, vh4, vh3, vh1);
        
        // Top face (reverse)
        mesh.AddFace(vh4, vh6, vh5, vh3);
        
        // Back face (reverse)
        mesh.AddFace(vh6, vh8, vh7, vh5);
        
        // Left side face (reverse) - fixed winding order
        mesh.AddFace(vh3, vh5, vh7, vh1);
        
        // Right side face (reverse)
        mesh.AddFace(vh8, vh6, vh4, vh2);
        
        // Bottom face (reverse)
        mesh.AddFace(vh7, vh8, vh2, vh1);

		if (_doubleSided)
		{
			
		}
    }

    private void UpdateMeshColliderVisibility()
    {
        if (!GameObject.IsValid)
            return;
            
        Collider collider = GameObject.Components.Get<Collider>();
        if (collider != null)
        {
            collider.Flags = _showMeshCollider ? ComponentFlags.None : ComponentFlags.Hidden;
        }
    }
}
