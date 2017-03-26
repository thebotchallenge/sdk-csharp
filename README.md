<p align="center">
  <a href="#0">
    <img src="https://raw.githubusercontent.com/vikebot/wiki/master/vikebot-repo-logo.png" height="120px">
  </a>
  <br>
  <strong>Official C# SDK for interacting with Vikebot Challenge, a competitive online coding game.</strong>
</p>

## Join your game
**Prerequirements:**
* Registered Vikebot Challenge account
* Joined a game at your dashboard
* Visual Studio solution
* Install `vikebot` over Nuget

**Start coding:**

Before we start, we need to add a `using vikebot;` to the head of your file. Afterwards we create a new `Game` instance, which will establish a connection to the host server, authorize you and upgrade from plain to AES encrypted communication. The constructor takes your `authCode` which can be copied from your dashboard.
```csharp
using vikebot;

class Example
{
    static void Main(string[] args)
    {
        using (Game g = new Game("yourAuthCode"))
        {
            int opponentsInMyArea = g.player.Radar();
        }
    }
}
```

### `player.Radar()`
This command can be used to determine the amount of people within the player's action area. The value is returned as `int`. The zone is a 11x11 matrix with the player in it's center.
<br><br><img src="img/radar.png" height="300px">
