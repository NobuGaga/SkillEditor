using UnityEngine;

[ExecuteInEditMode]
public class SkillEditorMono : MonoBehaviour {

    private Animator m_animator;

    /// <summary>
	/// unity run function in first time loading MB component(also component disactive)
	/// </summary>
	void Awake() {
        Debug.Log("SkillEditorMono::Awake");
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
            m_animator = gameObject.AddComponent<Animator>();
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

    public void AddAllAnimationClipName(string[] clips) {
        //string clipName = clips[0];
        //Debug.Log(clipName);
        //m_animator.Play(clipName);
    }

    public void Update(float deltaTime) {
        m_animator.Update(deltaTime);
    }
}