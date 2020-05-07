using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffect : MonoBehaviour
{
    public enum Mode
    {
        Horizontal = 0,
        Vertical = 1,
        Random = 2
    }
    public Color color;
    public Mode mode;
    public float brushSize = 0.1f;
    public float speed = 0.01f;

    private Material material;
    private uint bufferSize = 4096;
    private Vector2 position;
    private Vector2 moveVector;
    private int pathPoints = 0;

    private ComputeBuffer pathXBuffer;
    private ComputeBuffer pathYBuffer;

    void Awake()
    {
        material = new Material(Shader.Find("Hidden/Painter"));

        pathXBuffer = new ComputeBuffer((int)bufferSize, sizeof(float), ComputeBufferType.Default);
        pathYBuffer = new ComputeBuffer((int)bufferSize, sizeof(float), ComputeBufferType.Default);

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetBuffer("_pathX", pathXBuffer);
        material.SetBuffer("_pathY", pathYBuffer);
        material.SetVector("_color", color);
        material.SetFloat("_brushSize", brushSize);
        material.SetInt("_pathPoints", pathPoints);

        Graphics.Blit(source, destination, material);
    }
    // Start is called before the first frame update
    void Start()
    {
        switch (mode)
        {
            case Mode.Horizontal:
                position = new Vector2(1 + brushSize, 1 - brushSize);
                moveVector = new Vector2(-speed, 0.0f);
                break;
            case Mode.Vertical:
                position = new Vector2(1 - brushSize, 1 + brushSize);
                moveVector = new Vector2(0.0f, -speed);
                break;
            case Mode.Random:
                position = new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
                moveVector = new Vector2((UnityEngine.Random.value-0.5f)*speed, (UnityEngine.Random.value-0.5f)*speed);
                break;
        }

        InvokeRepeating("CalculateMovement", 0.0f, 0.01f);
    }

    void Step()
    {
        if(pathPoints == bufferSize)
        {
            bufferSize *= 2;
            pathXBuffer.SetCounterValue(bufferSize);
            pathXBuffer.SetCounterValue(bufferSize);
        }
        pathXBuffer.SetData(new float[] { position.x }, 0, pathPoints, 1);
        pathYBuffer.SetData(new float[] { position.y }, 0, pathPoints, 1);

        pathPoints++;
    }

    void CalculateMovement()
    {
        switch (mode)
        {
            case Mode.Horizontal:
                float positiveXDirection = moveVector.x > 0 ? 1.0f : -1.0f;
                moveVector.y = -brushSize/200.0f;
                
                if(Mathf.Abs(moveVector.x) != speed)
                {
                    moveVector.x = moveVector.x < 0 ? -speed : speed;
                }
                
                break;
            case Mode.Vertical:
                float positiveYDirection = moveVector.y > 0 ? 1.0f : -1.0f;
                moveVector.x = -brushSize / 200.0f;

                if (Mathf.Abs(moveVector.y) != speed)
                {
                    moveVector.y = moveVector.y < 0 ? -speed : speed;
                }
                break;
            case Mode.Random:
                moveVector.x = 0.8f * moveVector.x + (UnityEngine.Random.value - 0.5f) * speed;
                moveVector.y = 0.8f * moveVector.y + (UnityEngine.Random.value-0.5f) * speed;
                break;
        }
        position += moveVector;

        DetectEdgeCollision();
    }

    void DetectEdgeCollision()
    {
        {
            if (position.x >= (1.0f+brushSize) || position.x <= (0.0f-brushSize))
            {
                OnEdgeCollision(VerticalEdge: true);
            }
            if ((position.y-brushSize) >= 1.0f || (position.y + brushSize) <= 0.0f)
            {
                OnEdgeCollision(VerticalEdge: false);
            }
        }

    }

    void OnEdgeCollision(bool VerticalEdge)
    {
        if(VerticalEdge)
        {
            moveVector.x *= -1;

            switch (mode)
            {
                case Mode.Horizontal:
                    
                    break;
                case Mode.Vertical:
                    break;
                case Mode.Random:
                    moveVector.y = 0.0f;
                    break;
            }
        } else
        {
            moveVector.y *= -1;

            switch (mode)
            {
                case Mode.Horizontal:
                    break;
                case Mode.Vertical:
                    
                    break;
                case Mode.Random:
                    moveVector.x = 0.0f;
                    break;
            }
        }

        if(position.x >= 1.0f + brushSize / 2)
        {
            position.x = 1.0f + brushSize / 2;
        } else if(position.x < .0f - brushSize / 2)
        {
            position.x = .0f - brushSize / 2;
        }

        if (position.y >= 1.0f + brushSize / 2)
        {
            position.y = 1.0f + brushSize / 2;
        }
        else if (position.y < .0f - brushSize / 2)
        {
            position.y = .0f - brushSize / 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Step();
    }

    void OnDestroy()
    {
        pathXBuffer.Dispose();
        pathYBuffer.Dispose();
    }
}