﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VN_TextboxManager : MonoBehaviour
{
    [Tooltip("List of textbox data to pull from")]
    public List<TextboxData> AllTextboxData;

    // Current active TextboxData
    public TextboxData data;

    private VN_Manager manager;
    private Image textboxImage;
    private Image nameboxImage;
    private Image decorRTImage;
    private Image decorLBImage;

    private RectTransform decorRTRectTransform;
    private RectTransform decorLBRectTransform;

    public void Construct(VN_Manager manager)
    {
        this.manager = manager;

        textboxImage = manager.TextboxCanvas.GetComponent<Image>();
        nameboxImage = manager.NameCanvas.GetComponent<Image>();
        decorRTImage = manager.Decor_RTCanvas.GetComponent<Image>();
        decorLBImage = manager.Decor_LBCanvas.GetComponent<Image>();

        decorRTRectTransform = manager.Decor_RTCanvas.GetComponent<RectTransform>();
        decorLBRectTransform = manager.Decor_LBCanvas.GetComponent<RectTransform>();
    }

    public void SetTextboxData(TextboxData data, Sprite cornerDecor)
    {
        this.data = data;
        textboxImage.sprite = data.textboxSprite;
        textboxImage.SetNativeSize();

        nameboxImage.sprite = data.nameboxSprite;
        nameboxImage.SetNativeSize();

        if (data.FindCornerDecorSprite(cornerDecor))
        {
            var offsetPair = data.GetCornerDecorOffsets(cornerDecor);
            decorRTRectTransform.anchoredPosition = offsetPair.Item1;
            decorRTRectTransform.rotation = offsetPair.Item2;
            decorRTImage.sprite = cornerDecor;
            decorRTImage.SetNativeSize();

            Vector2 positionOffsetLB = new Vector2(-offsetPair.Item1.x, -offsetPair.Item1.y);
            Quaternion rotationOffsetLB = new Quaternion(0, 0, offsetPair.Item2.z - 180, 0);
            decorLBRectTransform.anchoredPosition = positionOffsetLB;
            decorLBRectTransform.rotation = rotationOffsetLB;
            decorLBImage.sprite = cornerDecor;
            decorLBImage.SetNativeSize();
        }
        else
        {
            Debug.LogError("Cannot find sprite \"" + cornerDecor.name + "\" in TextboxData \"" + data.name + "\"");
        }
    }

    public void SetDefaultData()
    {
        TextboxData data = AllTextboxData[0];
        Sprite sprite = data.cornerDecorList[0].sprite;
        SetTextboxData(data, sprite);
    }

}