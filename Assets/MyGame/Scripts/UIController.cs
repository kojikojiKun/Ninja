using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject m_sliders;
    [SerializeField] private Slider m_sliderLeft;
    [SerializeField] private Slider m_sliderRight;
    private SearchGauge m_gauge;

    private void Awake()
    {
        m_gauge = new SearchGauge();
    }

    public void ControlFillValue()
    {
        m_sliderLeft.value = m_gauge.CalcFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
        m_sliderRight.value = m_gauge.CalcFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
    }

    private void Update()
    {
        if(GameManager.s_Instance.HighestScoreEnemy==null)
            return;

        ControlFillValue();

        if (GameManager.s_Instance.HighestScoreEnemy.TotalScore <= 0)
            m_sliders.SetActive(false);
        else
            m_sliders.SetActive(true);
    }
}
