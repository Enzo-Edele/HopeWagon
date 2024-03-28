using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI headerField;
    [SerializeField] TextMeshProUGUI contentField;
    [SerializeField] LayoutElement layout;
    [SerializeField] int characterWarpLimit;
    [SerializeField] RectTransform rectTransform;
    public Animator animator;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layout.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit);
    }

    void Update()
    {
        if (Application.isEditor) { 
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;

            layout.enabled = (headerLength > characterWarpLimit || contentLength > characterWarpLimit);
        }

        Vector2 position = Input.mousePosition;

        float pivotX = position.x / Screen.width;
        float pivotY = position.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = position;
    }
}
