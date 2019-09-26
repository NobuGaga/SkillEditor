using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class SkillEditorMono : MonoBehaviour {

    private Animator m_animator;

    private Dictionary<string, AnimationClip> m_dicClip = new Dictionary<string, AnimationClip>();

    private bool m_isPlaying = false;
    private bool m_isLoop = false;
    private float m_curClipPlayTime;
    private float m_curClipTime;

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

    public void AddAllAnimationClipName(AnimationClip[] clips) {
        for (int i = 0; i < clips.Length; i++) {
            m_dicClip.Add(clips[i].name, clips[i]);
        }
    }

    public void Play() {
        foreach (var data in m_dicClip) {
            m_curClipTime = data.Value.length;
            m_curClipPlayTime = 0;
            m_isPlaying = true;
            m_isLoop = !data.Value.isLooping;
            m_animator.Play(data.Key);
            Debug.Log(string.Format("play clip name {0}, clip time {1}", data.Key, m_curClipTime));
            break;
        }
    }

    public void UpdateAnimation(float deltaTime) {
        if (!m_isPlaying)
            return;
        m_curClipPlayTime += deltaTime;
        if (m_curClipPlayTime < m_curClipTime)
            m_animator.Update(deltaTime);
        else if (m_isLoop)
            Play();
        else
            m_curClipPlayTime = 0;
    }
}