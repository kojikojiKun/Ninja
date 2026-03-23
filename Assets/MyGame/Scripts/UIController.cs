using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_sliderPivot;
    [SerializeField] private GameObject m_sliders;
    [SerializeField] private Slider m_sliderLeft;
    [SerializeField] private Slider m_sliderRight;
    private SearchGauge m_gauge;

    private void Awake()
    {
        m_gauge = new SearchGauge();
    }

    void ControlFillValue()
    {
        //スライダーの増え方が違うため、左右別で計算.
        m_sliderLeft.value = m_gauge.CalcLeftFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
        m_sliderRight.value = m_gauge.CalcRightFillValue(GameManager.s_Instance.HighestScoreEnemy.TotalScore);
    }

    void TurnForEnemy()
    {
        float angle = m_gauge.CalcAngleBetweenEnemyAndPlayer(
            GameManager.s_Instance.HighestScoreEnemy.gameObject.transform,
            Camera.main.transform
            );

        m_sliderPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        if (GameManager.s_Instance.HighestScoreEnemy == null)
            return;

        ControlFillValue();
        TurnForEnemy();

        if (GameManager.s_Instance.HighestScoreEnemy.TotalScore <= 0)
            m_sliders.SetActive(false);
        else
            m_sliders.SetActive(true);
    }
}
