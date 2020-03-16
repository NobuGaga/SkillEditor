@Echo off
xcopy "Assets/Editor/.luaconfig2/clientOnline/animclipconfig" "../Resources/lua/data/config/animclipconfig" /e/y
xcopy "Assets/Editor/.luaconfig2/clientOnline/animclipconfig" "Assets/Editor/.luaconfig2/serverOnline/animclipconfig" /e/y
@Echo on