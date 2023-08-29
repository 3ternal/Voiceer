![](https://user-images.githubusercontent.com/31283418/72233590-93c62d00-360b-11ea-91c4-f81e59e6f610.png)
# Voiceer
Voice + Cheer = Voiceer!

This is a Unity editor extension that will play a pre-recorded voice whenever you...
* Save
* Enter Play Mode
* Exit Play Mode
* Complete a build
* And more!

Demo video:
https://twitter.com/CST_negi/status/1214500926326628352

Developed in cooperation with: [Musubime Yui](https://twitter.com/musubimeyui)

## Installation Instructions
1. Download the repo.
1. In Unity, navigate to the Package Manager.
2. Click the plus button in the top-left corner.
3. Click "[add package from disk](https://docs.unity3d.com/Manual/upm-ui-local.html)" and select the "package.json" file.

Alternate option: Install from [OpenUPM](https://openupm.com/packages/com.negipoyoc.voiceer/).

## How to change the voice preset
1. Navigate to Window -> Voiceer -> Voice Preset Selector.

![](https://user-images.githubusercontent.com/31283418/72231862-08e03500-3601-11ea-9a1b-f9eadd6d99a7.png)

2. Click the button circled in red and select your desired preset.

![](https://user-images.githubusercontent.com/31283418/72231936-7ee49c00-3601-11ea-9c0b-b7da798ce87d.png)

3. That is all.

## How to generate your own voice preset
Navigate to Window -> Voiceer -> Voice Preset Generator.

![](https://user-images.githubusercontent.com/31283418/72231861-08479e80-3601-11ea-80f7-62ec8d60b182.png)

Here, you can either load an existing preset from a file, or create a new one.

![](https://user-images.githubusercontent.com/31283418/72231859-08479e80-3601-11ea-916c-b9ea6f917a88.png)

### Creating a new preset
1. Click on "create a new preset" (新規作成).

2. Open the window.

![New](https://user-images.githubusercontent.com/31283418/72231860-08479e80-3601-11ea-84d3-d92deb58e24e.png)

3. By clicking the plus (+) or minus (-) buttons, you can add or remove voice clips to a particular event.

![](https://user-images.githubusercontent.com/31283418/72231986-bd7a5680-3601-11ea-8f46-fec58664c17f.png)

4. That's all (note: your changes will be auto-saved).

### Load an existing preset
1. Click on the circle at the top and select a preset.

2. Just as when creating a new preset, you can use the plus (+) or minus (-) buttons to add or remove voice clips to a particular event.

3. That's all (note: your changes will be auto-saved).

## Other Buttons
### Return to Preset Selection Mode (Preset選択モードに戻る)
You will return to Preset Selection Mode, and the editing window will be reset.

# Voice Package Creation
1. Navigate to Window -> Voiceer -> Voice Preset Generator.
2. Select your preset.
3. Click on the "Create Package" button at the very bottom (パッケージを出力する).
4. That's all.

## About Voice Files
* The voice files are located in "Assets/Voiceer/VoiceResources/\<Preset Name\>". Feel free to use them.
* .wav files are recommended.

# Operating Environment
Confirmed: Unity 2018-2022

Later versions are expected to work as well.

-----------------------------------

# Voiceer
Voice+Cheer=Voiceer(ぼいしあ)

Unityで
* セーブしたとき
* 再生したとき
* 再生を終了した時
* ビルドした時
…など、あらゆる状態変化をフックし、事前に登録した音声を再生するEditor拡張です。

デモ動画：
https://twitter.com/CST_negi/status/1214500926326628352

開発協力：[結目ユイ様](https://twitter.com/musubimeyui)

## 使い方
1. [Release](https://github.com/negipoyoc/Voiceer/releases)より、Voiceer+Sample.unitypackageをダウンロードします。
2. プロジェクトにインポートします。
3. 終わりです。これでボイスが再生されます。

## 違うボイスPresetを試したい時
1.Window->Voiceer->Voice Preset Selectorを選択します。
![](https://user-images.githubusercontent.com/31283418/72231862-08e03500-3601-11ea-9a1b-f9eadd6d99a7.png)

2.赤丸の部分をクリックし、Presetを選択します。
![](https://user-images.githubusercontent.com/31283418/72231936-7ee49c00-3601-11ea-9c0b-b7da798ce87d.png)

3.終わりです。

## ボイスの種類を自分で編集したい時
Window->Voiceer->Voice Preset Generatorを選択します。
![](https://user-images.githubusercontent.com/31283418/72231861-08479e80-3601-11ea-80f7-62ec8d60b182.png)

ここでは、既存のファイルをロード、または、新規作成を選べます。
![](https://user-images.githubusercontent.com/31283418/72231859-08479e80-3601-11ea-916c-b9ea6f917a88.png)

### 新規作成
1. 新規作成を押します。

2. 画面が開きます。

![New](https://user-images.githubusercontent.com/31283418/72231860-08479e80-3601-11ea-84d3-d92deb58e24e.png)

3. [+][-]ボタンで、任意のフックでボイスを追加したり消したりします。

![](https://user-images.githubusercontent.com/31283418/72231986-bd7a5680-3601-11ea-8f46-fec58664c17f.png)

4. 終わりです。(自動セーブされているので注意)

### 既存ファイルのロード
1. 一番上行の○をクリックして選択します。

2. 新規作成のとき同様、[+][-]ボタンで、任意のフックでボイスを追加したり消したりします。

3. 終わりです。(自動セーブされているので注意)

## その他
### Preset選択モードに戻る
Presetの選択モードに戻ります。(編集画面が初期化されます。)

# ボイスのパッケージを出力する場合(ボイス製作者さまへ)
1. Window->Voiceer->Voice Preset Generatorを選択します。
2. Presetを選択します。
3. Windowの一番下に[パッケージを出力する]というボタンがあるのでこれをクリックします。
4. 終わりです。

## ボイスファイルについて
* ボイスはAssets/Voiceer/VoiceResources/{プリセット名}以下に置いておくと、みんなが幸せになれます。
* wavファイルを推奨しています。

# 動作環境
Unity2018.4 2019.1 2019.2 2019.3, 2020, 2021
で動作することを確認しています。
