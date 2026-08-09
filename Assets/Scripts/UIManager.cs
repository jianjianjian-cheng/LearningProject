using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum UILayer
{
    HUD,        // 常驻层（血条、小地图）
    Normal,     // 普通面板（背包、设置）
    Popup,      // 弹窗层（确认对话框）
    Toast       // 最顶层提示
}

// 面板类型枚举（每加一个面板加一项）
public enum PanelType
{
    Inventory,
    Reward,
}
public class UIManager : MonoBehaviour
{
    public static UIManager Instacnce { get; private set; }

    [SerializeField] private Transform hudLayer;
    [SerializeField] private Transform normalLayer;
    [SerializeField] private Transform popupLayer;
    [SerializeField] private Transform toastLayer;

    //面板的路径
    private readonly Dictionary<PanelType, string> panelPaths = new Dictionary<PanelType, string>
    {
        {PanelType.Inventory , "UI/Panel/InventoryPanel"},
        {PanelType.Reward , "UI/Panel/RewardPanel"},
    };

    //已经加载的面板缓存
    private readonly Dictionary<PanelType, UIPanelBase> panelCache = new Dictionary<PanelType, UIPanelBase>();
    //已经打开的面板
    private readonly List<UIPanelBase> openPanels = new List<UIPanelBase>();

    void Awake()
    {
        Instacnce = this;
    }


    //根据层级获取容器
    private Transform getLayerContainer(UILayer layer)
    {
        return layer switch
        {
            UILayer.HUD => hudLayer,
            UILayer.Normal => normalLayer,
            UILayer.Popup => popupLayer,
            UILayer.Toast => toastLayer,
            _ => normalLayer,
        };
    }

    //获取层级
    private UILayer getPanelLayer(PanelType panelType)
    {
        return panelType switch
        {
            PanelType.Inventory => UILayer.Normal,
            PanelType.Reward => UILayer.Normal,
            _ => UILayer.Normal,
        };
    }


    //显示面板
    public T Show<T>(object data = null) where T : UIPanelBase
    {
        PanelType type = ParceType<T>();
        //先检测是否有缓存
        if (!panelCache.ContainsKey(type))
        {
            GameObject prefab = Resources.Load<GameObject>(panelPaths[type]);
            if (prefab == null) return null;
            GameObject go = Instantiate(prefab);
            go.name = type.ToString();
            go.transform.SetParent(getLayerContainer(getPanelLayer(type)), false);
            panelCache[type] = go.GetComponent<T>();
            panelCache[type].OnInit(data);
            panelCache[type].UILayer = getPanelLayer(type);
        }
        panelCache[type].Show();

        if (panelCache[type].UILayer == UILayer.Normal
        || panelCache[type].UILayer == UILayer.Popup)
        {
            openPanels.Add(panelCache[type]);
        }

        return (T)panelCache[type];
    }

    public void Hide<T>() where T : UIPanelBase
    {
        PanelType type = ParceType<T>();
        if (!panelCache.ContainsKey(type)) return;
        //出栈
        if (openPanels.Count > 0)
        {
            openPanels.Remove(panelCache[type]);
            panelCache[type].Hide();
        }
    }

    /// <summary>
    /// 关闭顶部面板
    /// </summary>
    public void CloseTopPanel()
    {
        if (openPanels.Count > 0)
        {
            var panel = openPanels[openPanels.Count - 1];
            panel.Hide();
            openPanels.Remove(panel);
        }
    }

    /// <summary>
    /// 关闭所有打开的面板
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (var panel in openPanels)
        {
            panel.Hide();
        }
        openPanels.Clear();
    }



    /// <summary>
    /// 工具脚本,根据泛型推断PnaelTyoe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="System.Exception"></exception>
    private PanelType ParceType<T>() where T : UIPanelBase
    {
        string typeName = typeof(T).Name.Replace("Panel", "");

        if (System.Enum.TryParse<PanelType>(typeName, out var type))
            return type;

        throw new System.Exception($"未注册的面板类型: {typeof(T).Name}");
    }
}

