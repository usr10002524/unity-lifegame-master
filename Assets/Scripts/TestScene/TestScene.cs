using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// テストシーン
/// </summary>
public class TestScene : MonoBehaviour
{
    [SerializeField] private GameObject dropDownObject;
    [SerializeField] private UnityEvent<string> onDropDownChanged;

    private TMP_Dropdown dropdown;
    private bool doropdownInitialized;
    private List<string> dropdownItems;
    private static readonly string initialItem = "Select Pattern";

    // Start is called before the first frame update
    void Start()
    {
        dropdown = dropDownObject.GetComponent<TMP_Dropdown>();


    }

    // Update is called once per frame
    void Update()
    {
        if (!doropdownInitialized)
        {
            if (PatternLoadManager.Instance.IsLoadCompleted())
            {
                InitDropdown();
            }
        }
    }

    /// <summary>
    /// ドロップダウンの初期化
    /// </summary>
    private void InitDropdown()
    {
        if (doropdownInitialized)
        {
            return;
        }

        if (dropdown != null)
        {
            dropdown.ClearOptions();

            dropdownItems = PatternLoadManager.Instance.GetPatternList();
            dropdownItems.Insert(0, initialItem);
            dropdown.AddOptions(dropdownItems);
        }



        doropdownInitialized = true;
    }

    /// <summary>
    /// ドロップダウンメニューの選択を変更した際に呼ばれる
    /// </summary>
    /// <param name="value">選択他アイテム</param>
    public void OnDropdownValueChanged(int value)
    {
        if (value < dropdownItems.Count)
        {
            string name = dropdownItems[value];
            // Debug.Log(string.Format("DropdownValueChanged() value:{0} name:{1}", value, name));

            if (value > 0)
            {
                onDropDownChanged.Invoke(name);
            }
        }
    }
}
