using UnityEngine;

[ExecuteInEditMode]
public class SkillEditorMono : MonoBehaviour {

    private Animation m_animation;

    /// <summary>
	/// unity run function in first time loading MB component(also component disactive)
	/// </summary>
	void Awake() {
		Debug.Log("SkillEditorMono::Awake");
        m_animation = GetComponent<Animation>();
        if (m_animation == null)
            m_animation = gameObject.AddComponent<Animation>();
        m_animation.playAutomatically = false;
    }

    /// <summary>
    /// in first time loading MB component after OnEnable()
    /// </summary>
    void Start() {
		Debug.Log("SkillEditorMono::Start");
	}

	/// <summary>
	/// destroy node or function run function after OnDisable()
	/// </summary>
	void OnDestroy() {
		Debug.Log("SkillEditorMono::OnDestroy");
	}
}