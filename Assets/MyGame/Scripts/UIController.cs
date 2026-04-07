using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_sliderPivot;
    [SerializeField] private GameObject m_sliders;
    [SerializeField] private Slider m_sliderLeft;
    [SerializeField] private Slider m_sliderRight;
    [SerializeField, Range(0, 100f)] private float m_percentageOfEnableSlider_Max;
    [SerializeField, Range(0, 100f)] private float m_percentageOfEnableSlider_Min;
    private EnemyController m_highScoreEnemy;
    private SearchGauge m_gauge;
    private Camera m_camera;

    private void Awake()
    {
        m_gauge = new SearchGauge(m_sliderPivot);

        //パーセントを少数になおす。
        if (m_percentageOfEnableSlider_Max > 0f)
            m_percentageOfEnableSlider_Max /= 100f;
        if (m_percentageOfEnableSlider_Min > 0f)
            m_percentageOfEnableSlider_Min /= 100;
    }

    public void OnUpdateHighScoreEnemy(EnemyController highScoreEnemy)
    {
        m_highScoreEnemy = highScoreEnemy;
    }

    void ControlSearchGaugeFillValue()
    {
        //スライダーの増え方が違うため、左右別で計算.
        m_sliderLeft.value = m_gauge.CalcLeftFillValue(m_highScoreEnemy.TotalScore);
        m_sliderRight.value = m_gauge.CalcRightFillValue(m_highScoreEnemy.TotalScore);
        m_gauge.TurnToEnemy(
                   m_highScoreEnemy.transform,
                   m_camera
                   );

        //スライダーのValueが閾値におさまっている間はスライダーを表示する.
        float leftPercentage = m_sliderLeft.value / m_sliderLeft.maxValue;
        float rightPercentage = m_sliderRight.value / m_sliderRight.maxValue;
        if (
            leftPercentage <= m_percentageOfEnableSlider_Min || leftPercentage >= m_percentageOfEnableSlider_Max ||
            rightPercentage <= m_percentageOfEnableSlider_Min || rightPercentage >= m_percentageOfEnableSlider_Max
            )
            m_sliders.SetActive(false);
        else
            m_sliders.SetActive(true);
    }

    private void Update()
    {
        if (m_camera == null || m_highScoreEnemy == null)
            return;

        ControlSearchGaugeFillValue();
    }
}
