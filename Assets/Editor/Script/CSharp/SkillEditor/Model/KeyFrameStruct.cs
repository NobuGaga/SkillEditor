using System.Text;

namespace SkillEditor.Structure {

    using LuaTableKeyValue = LuaFormat.LuaTableKeyValue;

    internal enum KeyFrameLuaLayer {
        EnterTable = 0,
        Model = 1,
        Clip = 2,
        FrameIndex = 3,
        FrameData = 4,
        CustomData = 5,
        Effect = 6,
        Rect = 7,
    }

    internal struct KeyFrameData {
        public string modelName;
        public SkillClipData[] clipDatas;

        public KeyFrameData(string modelName, SkillClipData[] clipDatas) {
            this.modelName = modelName;
            this.clipDatas = clipDatas;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.ClipDataStringLength);
        public override string ToString() {
            string clipDatasString = string.Empty;
            if (clipDatas != null) {
                m_staticBuilder.Clear();
                for (int index = 0; index < clipDatas.Length; index++)
                    m_staticBuilder.Append(clipDatas[index].ToString());
                clipDatasString = m_staticBuilder.ToString();
            }
            string tabString = Tool.GetTabString(KeyFrameLuaLayer.Model);
            string format = string.Intern("{0}[{1}] = {2}\n{3}{0}{4},\n");
            string toString = string.Format(format, tabString, modelName,
                                            LuaFormat.CurlyBracesPair.start,
                                            clipDatasString, LuaFormat.CurlyBracesPair.end);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct SkillClipData {
        public string clipName;
        public FrameData[] frameDatas;

        public SkillClipData(string clipName, FrameData[] frameDatas) {
            this.clipName = clipName;
            this.frameDatas = frameDatas;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.FrameDataStringLength);
        public override string ToString() {
            string frameDataString = string.Empty;
            string tabString = Tool.GetTabString(KeyFrameLuaLayer.FrameIndex);
            string format;
            if (frameDatas != null) {
                m_staticBuilder.Clear();
                m_staticBuilder.Append(LuaFormat.LineSymbol);
                format = string.Intern("{0}[{1}] = {2}\n{3}{0}{4},\n");
                for (int index = 0; index < frameDatas.Length; index++) {
                    FrameData data = frameDatas[index];
                    string formatString = string.Format(format, tabString, data.frameIndex,
                                                        LuaFormat.CurlyBracesPair.start, data.ToString(),
                                                        LuaFormat.CurlyBracesPair.end);
                    m_staticBuilder.Append(formatString);
                }
                frameDataString = m_staticBuilder.ToString();
            }
            format = string.Intern("{0}[\"{1}\"] = {2}{3}{0}{4},\n");
            tabString = Tool.GetTabString(KeyFrameLuaLayer.Clip);
            string toString = string.Format(format, tabString, clipName, LuaFormat.CurlyBracesPair.start,
                                            frameDataString, LuaFormat.CurlyBracesPair.end);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal struct FrameData : ITable {
        public int frameIndex;
        public string name;
        public float time;
        public short priority;
        public CustomData[] customData;

        public FrameData(int frameIndex, string name, float time, short priority, CustomData[] customData) {
            this.frameIndex = frameIndex;
            this.name = name;
            this.time = time;
            this.priority = priority;
            this.customData = customData;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.CustomDataStringLength);
        public override string ToString() {
            string customDataString = string.Empty;
            string tabString = Tool.GetTabString(KeyFrameLuaLayer.FrameData);
            if (customData != null) {
                m_staticBuilder.Clear();
                m_staticBuilder.Append(LuaFormat.LineSymbol);
                for (int index = 0; index < customData.Length; index++)
                    m_staticBuilder.Append(customData[index].ToString());
                m_staticBuilder.Append(tabString);
                customDataString = m_staticBuilder.ToString();
            }
            string format = string.Intern("{0}name = \"{1}\",\n{0}time = {2},\n{0}priority = {3},\n{0}data = {4}{5}{6},\n");
            string toString = string.Format(format, tabString, name, time, priority,
                                            LuaFormat.CurlyBracesPair.start,
                                            customDataString, LuaFormat.CurlyBracesPair.end);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Name:
                    name = value as string;
                    return;
                case Key_Time:
                    time = (float)value;
                    return;
                case Key_Priority:
                    priority = (short)(int)value;
                    return;
                case Key_Data:
                    customData = value as CustomData[];
                    return;
            }
        }

        private const string Key_Name = "name";
        private const string Key_Time = "time";
        private const string Key_Priority = "priority";
        private const string Key_Data = "data";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[4];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Name, LuaFormat.Type.LuaString);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Time, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Priority, LuaFormat.Type.LuaInt);
            m_arraykeyValue[3] = new LuaTableKeyValue(Key_Data, LuaFormat.Type.LuaTable);
            return m_arraykeyValue;
        }
    }

    internal struct CustomData {
        public EFrameType frameType;
        public object data;

        public CustomData(EFrameType frameType, object data) {
            this.frameType = frameType;
            this.data = data;
        }

        private static readonly StringBuilder m_staticBuilder = new StringBuilder(Config.RectDataStringLength);
        public override string ToString() {
            string dataString = string.Empty;
            string tabString;
            string format;
            if (data is GrabData)
                dataString = ((GrabData)data).ToString();
            else if (data is UngrabData)
                dataString = ((UngrabData)data).ToString();
            else if (data is HarmData[]) {
                m_staticBuilder.Clear();
                tabString = Tool.GetTabString(KeyFrameLuaLayer.Effect);
                HarmData[] array = data as HarmData[];
                format = string.Intern("{0}[{1}] = {2}\n{3}{0}{4},\n");
                for (int index = 0; index < array.Length; index++)
                    m_staticBuilder.Append(string.Format(format, tabString, index + 1,
                                                LuaFormat.CurlyBracesPair.start, array[index].ToString(),
                                                LuaFormat.CurlyBracesPair.end));
                dataString = m_staticBuilder.ToString();
            }
            tabString = Tool.GetTabString(KeyFrameLuaLayer.CustomData);
            format = string.Intern("{0}[{1}] = {2}\n{3}{0}{4},\n");
            string toString = string.Format(format, tabString, (int)frameType,
                                            LuaFormat.CurlyBracesPair.start,
                                            dataString, LuaFormat.CurlyBracesPair.end);
            string internString = string.Intern(toString);
            return internString;
        }
    }

    internal enum EFrameType {
        None = 0,
        Grab = 1,
        Ungrab = 2,
        Collision = 3,
        Harm = 4,
        Max = 5,
    }

    internal struct GrabData : ITable {
        public short type;
        public int id;

        public GrabData(short type, int id) {
            this.type = type;
            this.id = id;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(KeyFrameLuaLayer.Effect);
            string format = string.Intern("{0}type = {1},\n{0}id = {2},\n");
            string toString = string.Format(format, tabString, type, id);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (short)(int)value;
                    return;
            }
        }

        private const string Key_Type = "type";
        private const string Key_Id = "id";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[2];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Type, LuaFormat.Type.LuaInt);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Id, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal struct UngrabData : ITable {
        public short type;
        public int id;

        public UngrabData(short type, int id) {
            this.type = type;
            this.id = id;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(KeyFrameLuaLayer.Effect);
            string format = string.Intern("{0}type = {1},\n{0}id = {2},\n");
            string toString = string.Format(format, tabString, type, id);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_Type:
                    type = (short)(int)value;
                    return;
                case Key_Id:
                    id = (short)(int)value;
                    return;
            }
        }

        private const string Key_Type = "type";
        private const string Key_Id = "id";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[2];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_Type, LuaFormat.Type.LuaInt);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Id, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal struct HarmData : ITable {
        public float x;
        public float y;
        public float z;
        public float width;
        public float height;
        public short depth;

        public HarmData(float x, float y, float z, float width, float height, short depth) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public override string ToString() {
            string tabString = Tool.GetTabString(KeyFrameLuaLayer.Rect);
            string format = string.Intern("{0}x = {1},\n{0}y = {2},\n{0}z = {3},\n{0}width = {4},\n{0}height = {5},\n{0}depth = {6},\n");
            string toString = string.Format(format, tabString, x, y, z, width, height, depth);
            string internString = string.Intern(toString);
            return internString;
        }

        public void SetTableKeyValue(string key, object value) {
            switch (key) {
                case Key_X:
                    x = (float)value;
                    return;
                case Key_Y:
                    y = (float)value;
                    return;
                case Key_Z:
                    z = (float)value;
                    return;
                case Key_Width:
                    width = (float)value;
                    return;
                case Key_Height:
                    height = (float)value;
                    return;
                case Key_Depth:
                    depth = (short)(int)value;
                    return;
            }
        }

        private const string Key_X = "x";
        private const string Key_Y = "y";
        private const string Key_Z = "z";
        private const string Key_Width = "width";
        private const string Key_Height = "height";
        private const string Key_Depth = "depth";

        private static LuaTableKeyValue[] m_arraykeyValue;
        public LuaTableKeyValue[] GetTableKeyValueList() {
            if (m_arraykeyValue != null)
                return m_arraykeyValue;
            m_arraykeyValue = new LuaTableKeyValue[6];
            m_arraykeyValue[0] = new LuaTableKeyValue(Key_X, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[1] = new LuaTableKeyValue(Key_Y, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[2] = new LuaTableKeyValue(Key_Z, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[3] = new LuaTableKeyValue(Key_Width, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[4] = new LuaTableKeyValue(Key_Height, LuaFormat.Type.LuaNumber);
            m_arraykeyValue[5] = new LuaTableKeyValue(Key_Depth, LuaFormat.Type.LuaInt);
            return m_arraykeyValue;
        }
    }

    internal interface ITable {

        void SetTableKeyValue(string key, object value);
        LuaTableKeyValue[] GetTableKeyValueList();
    }
}