

public static class SkillEditorKeyFrameModel {

    private static SkillEditorKeyFrameData m_data = new SkillEditorKeyFrameData();

    public static string ModelName {
        set { m_data.modelName = value; }
        get { return m_data.modelName; }
    }
}