using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_LG_GateEnd : UIC_LG_Gate
{
    protected override void Awake()
    {
        base.Awake();
        _image = GetComponent<Image>();
        _text = transform.GetChild(0).GetComponentInChildren<Text>();
    }

    void Update()
    {
        if (outValue == 1)
        {
            Color outColor = Color.green;
            ColorUtility.TryParseHtmlString("#6AFF6A", out outColor);
            _image.color = outColor;
            _text.text = "on";
        }
        else if (outValue == 0)
        {
            Color outColor = Color.green;
            ColorUtility.TryParseHtmlString("#636363", out outColor);
            _image.color = outColor;
            _text.text = "off";
        }
        else
        {
            _image.color = new Color(1, 1, 1);
            _text.text = "--";
        }
    }

    Image _image;
    Text _text;

    public override void Solve()
    {
        GetInputs();

        if (inputs.Count > 0)
        {
            outValue = inputs[0];
        }
        else
        {
            outValue = -1;
            return;
        }
    }
}
