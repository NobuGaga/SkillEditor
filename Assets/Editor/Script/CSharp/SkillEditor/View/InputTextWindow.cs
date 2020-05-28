
namespace SkillEditor {

    internal class InputTextWindow : BaseEditorWindow {

        private const string WindowName = "输入动画 ID 窗口";
        private const string LabelID = "动画 ID";
        private const string BtnConfirm = "Confirm";

        private static uint m_inputID = 0;

        public static void Open() => Open<InputTextWindow>(WindowName);

        private void OnGUI() => CenterLayoutUI(WindowUI);

        private void WindowUI() {
            BeginVertical(Layout.Top);

            BeginHorizontal(Layout.Left);
            SpaceWithLabel(LabelID);
            m_inputID = TextField(m_inputID);
            EndHorizontal(Layout.Left);
            
            if (Button(BtnConfirm)) {
                Controller.AddNewClipGroupData(m_inputID);
                GetWindow<InputTextWindow>().Close();
                return;
            }

            EndVertical(Layout.Top);
        }
    }
}