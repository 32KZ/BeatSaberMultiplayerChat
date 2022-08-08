﻿# TODOs

Phase 1: Text. Phase 2: Voice.

## General / shared
- [X] Settings UI

## Text 

- [X] Chat box UI in lobby
    - [X] Lobby tab / button
      - [X] "Unread" count/indicator
    - [X] Scrollable container
      - [X] Keep player list open 
      - [X] Fix auto scroll
      - [X] Keyboard entry / send action
      - [X] Make it look decent
- [X] Chat bubbles
- [X] Sound notification

## Voice 

- [X] Device selection
- [X] Microphone capture
- [X] Sample / Encode / UnityOpus 
- [X] Voice packets
- [ ] Push-to-talk / mic toggle
- [ ] Voice activation volume
- [X] Mute players
  - [X] Player list cell integration with native UI
- [ ] Local activity indicator
- [ ] Remote activity indicator

## QA

- [ ] *Maybe:* Typing indicators
- [ ] 🪲 *Bug:* Leaving MpEx settings flow coordinator will break the chat title button (activation event never triggers for the lobby setup view?)
- [ ] *Less important*: Chat view will lag if *left open only* with lots of messages piling up; older game objects might need to be removed (cap to messages buffer size).