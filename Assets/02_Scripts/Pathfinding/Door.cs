using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Door : MonoBehaviour
{
    [SerializeField] private Sprite closed;
    [SerializeField] private Sprite opened;

    [SerializeField] private bool isOpen = false;
    public bool IsOpen => isOpen;

    private SpriteRenderer _doorSprite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _doorSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetTheDoor();
    }

    private void Open()
    {
        isOpen = true;
    }
    private void Close()
    {
        isOpen = false;
    }
    private void SetTheDoor()
    {
        _doorSprite.sprite = isOpen ? opened : closed;
    }
}
