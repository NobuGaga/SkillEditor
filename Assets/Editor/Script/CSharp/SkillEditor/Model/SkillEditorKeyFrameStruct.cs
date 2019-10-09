public struct SkillEditorKeyFrameData {
    public string modelName;
    public SkillEditorClipData clipData;
}

public struct SkillEditorClipData {
    public SkillEditorFrameData[] frameDatas;
}

public struct SkillEditorFrameData {
    public string name;
    public float time;
    public short priority;
    public SkillEditorCustomData customData;
}

public struct SkillEditorCustomData {
    public SkillEditorEFrameType frameType;
    public object data;
}

public enum SkillEditorEFrameType {
    None = 0,
    Grab = 1,
    Ungrab = 2,
    Collision = 3,
    Harm = 4,
    Max = 5,
}

public struct SkillEditorGrabData {
    public short type;
    public int id;
}

public struct SkillEditorUngrabData {
    public short type;
    public int id;
}

public struct SkillEditorHarmData {
    public float x;
    public float y;
    public float z;
    public float width;
    public float height;
    public short depth;
}