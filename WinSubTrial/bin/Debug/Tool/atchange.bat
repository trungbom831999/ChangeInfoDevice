adb uninstall com.atchange.pro
adb install -t E:\Android\Project\ATChangeCoding\app\build\outputs\apk\debug\app-debug.apk
adb shell pm grant com.atchange.pro android.permission.CHANGE_CONFIGURATION
adb shell am start -n com.atchange.pro/.MainActivity
adb shell su -c "killall zygote"