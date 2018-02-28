# TwitchChatPlugin

[1]:https://github.com/longod/TwitchChatPlugin
[2]:http://www.okayulu.moe
[3]:https://www.twitch.tv
[4]:https://github.com/longod/TwitchChatPlugin/releases
[5]:https://github.com/longod/TwitchChatPlugin/wiki/OAuth

[TwitchChatPlugin][1] は、[ゆかりねっと][2]のプラグインです。  
ゆかりねっとによって音声認識された文章を、[Twitch][3]の自身のチャンネルに対して発言することが出来ます。

[TwitchChatPlugin][1] is a [Yukarinette][2] plugin.  
This sends speech recognition results to [Twitch][3]'s chat in your channel.

English description is [below](#installation).

# インストール

[TwitchChatPlugin のダウンロード][4]

1. [ゆかりねっと][2]をダウンロードしてインストールする
1. [TwitchChatPlugin][4] をダウンロードして解凍する
1. TwitchChatPlugin の Pluginsフォルダの中にある、全てのDLLファイルをゆかりねっとインストール先のPluginsフォルダにコピーする
    * 標準インストール先: C:\Program Files (x86)\OKAYULU STYLE\ゆかりねっと\Plugins

# つかいかた

## クイックスタート

1. ゆかりねっとを起動する
1. プラグインメニューを選択する
1. TwitchChatPlugin を探す
1. 設定を開く
1. Twitchのユーザ名とOAuth（[後述](#oauth%E3%81%AE%E5%85%A5%E6%89%8B)）を入力し、OKを押す
1. 音声認識メニューから TwitchChatPlugin を有効にする
1. 開始する

## OAuthの入手

より詳細な手順は [こちら](https://github.com/longod/TwitchChatPlugin/wiki/OAuth)

### 概要

1. トークン生成サイトを開く https://twitchapps.com/tmi/
1. Connect および Authorize を行う
1. トークンを設定ウインドウにコピーペーストする
    * **トークンは誰にも教えないで下さい**

<br />

---

<br />

# Installation

[Download TwitchChatPlugin][4]

1. Get and Install [Yukarinette][2]
1. Get [TwitchChatPlugin][4]
1. Copy all dlls in TwitchChatPlugin's plugin directory to Yukarinette's plugin direcotry
    * Default installed location: C:\Program Files (x86)\OKAYULU STYLE\ゆかりねっと\Plugins

# How to Use

## Quick Start

1. Launch Yukarinette.
1. Click Plugin menu.
1. Find TwitchChatPlugin.
1. Open Settings.
1. Enter Twitch username and OAuth ([below seciton](#getting-oauth)), then OK.
1. Enable TwitchChatPlugin
1. Start Recognition

## Getting OAuth

For details, see [here](https://github.com/longod/TwitchChatPlugin/wiki/OAuth).

### Summary

1. Go to https://twitchapps.com/tmi
1. Connect and authorize
1. Copy and paste the token to the settings window
    * **DONT TELL ANYONE THE TOKEN**

<br />

---

<br />

# FAQ

* Find bugs?
    * Please report or contribute.

# Thanks

* [Yukarinette][2]
* [Twitch][3]
* [TwitchLib](https://github.com/TwitchLib/TwitchLib)
* [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)

## Websites

* [Black Masquerade Games](http://blackmasqueradegames.com)
* [TwitchChatPlugin](https://github.com/longod/TwitchChatPlugin)
* [TwitchChatPlugin's Wiki](https://github.com/longod/TwitchChatPlugin/wiki)

## TODO

* write docs how to use and revoke authorization
* latency for streaming lag
* allow only numeric on port text field
* spam limitation (20 or 100 times per 30 seconds)
* bad word filtering
* more security
* catch exceptions
* coverage mock and test
    * wrap logger for testing
    * using dry-run

# License

MIT License
