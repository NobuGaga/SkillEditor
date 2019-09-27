using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SkillEditorMono : MonoBehaviour {

    private Animator m_animator;

    private bool m_isPlaying = false;
    private bool m_isLoop = false;
    private float m_curClipPlayTime;
    private float m_curClipTime;

    /// <summary>
	/// unity run function in first time loading MB component(also component disactive)
	/// </summary>
	void Awake() {
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
            m_animator = gameObject.AddComponent<Animator>();
    }


    /// <summary>
    /// in first time loading MB component after OnEnable()
    /// </summary>
    void Start() {

	}

	/// <summary>
	/// destroy node or function run function after OnDisable()
	/// </summary>
	void OnDestroy() {

	}

    public void Play(string clipName, AnimationClip clip) {
        m_curClipTime = clip.length;
        m_curClipPlayTime = 0;
        m_isPlaying = true;
        m_isLoop = clip.isLooping;
        m_animator.Play(clipName);
        Debug.Log(string.Format("play clip name {0}, clip time {1}", clipName, m_curClipTime));
    }

    public void UpdateAnimation(float deltaTime) {
        if (!m_isPlaying)
            return;
        m_curClipPlayTime += deltaTime;
        if (m_curClipPlayTime < m_curClipTime)
            m_animator.Update(deltaTime);
        else if (m_isLoop)
            m_curClipPlayTime = 0;
        else {
            m_curClipPlayTime = 0;
            m_isPlaying = false;
        }
    }
}