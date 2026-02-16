using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider m_searchGauge;
    private SearchGauge m_search;

    private void Awake()
    {
        m_search = new SearchGauge();
    }
}
