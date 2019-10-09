namespace SkillEditor {

    public static class KeyFrameModel {

        private static KeyFrameData m_data = new KeyFrameData();

        public static string ModelName {
            set { m_data.modelName = value; }
            get { return m_data.modelName; }
        }
    }
}