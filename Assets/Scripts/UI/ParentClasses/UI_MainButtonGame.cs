using UnityEngine;

public class UI_MainButtonGame : MonoBehaviour
{
    protected GwentCard m_myData;
    public virtual GwentCard CardData { get { return m_myData; } set { m_myData = value; } }

    protected int m_myOrder;
    public virtual int CardOrder { get { return m_myOrder; } set { m_myOrder = value; } }

}
