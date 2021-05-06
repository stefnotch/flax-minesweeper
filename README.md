# flax-minesweeper
A smol Minesweeper game using the amazing [FlaxEngine](http://flaxengine.com/).





## Tweening Design Doc

### Done

- tween to position & REPLACE/CANCEL the existing tweens
  ==> Minesweeper animations (at the corners)

- tween "group/sequence" (maybe a .chain() function?)
  ==> cancelling chained tweens (e.g. that pillar tween sequence)
  ==> saying: I want to go to the middle of the sequence (0.5)
  ==> paths

- chain tweens
  ==> nice animations (pillar coming out, particles ==> rotating, other particles ==> opening the top)

- TweenSettings
- Optional StartDelay
  ==> Sequence: Take the last element of the sequence
  ==> Action: Take the endtime of that action

### Todo

- tween into another tween (move from A to B and then change the target)

- delta/additive tweens: tween from A(0,0,0) to B (0,10,0) and moving(linearvelocity/tweening) from A(0,0,0) to C(0,0,10) ==> end: D (0,10,10)
  ==> animating moving objects
- tween set percentage
- optimized tween stuff that can get called every Update()
  ==> animating a gun turrent (move it to the mouse every Update())
- constant speed animations (as opposed to constant time)
  ==> paths
- staggered effects
  ==> different objects, same effect (with a delay)
      repeat(100) + stagger(10) // milliseconds/seconds, whatever Flax uses
- randomized start times
  ==> tween 100 blocks into the start position
  ==> (combined with staggering)
- tweening towards a moving target
  ==> Tweening to the mouse
- tween from float 1 to float 7
  ==> progress bar (custom onUpdate)
- tween forever
  ==> all animations that should play forever
- tween the width
  ==> object gets thinner when you move it quickly
- tween binding
  ==> speed --> bound to width
  ==> quite possible, since it's not really binding, it just grabs the current speed every Update()
- https://www.youtube.com/watch?v=Fy0aCDmgnxg
- Use `Time.GameTimeTicks` (which is a long) to avoid float precision issues
- Check out https://github.com/honzapatCZ/FTween
- Use closures (e.g. end position as a captured variable so that it can be changed dynamically)



- average tween: tween from A(0,0,0) to B (0,10,0) and tween from A(0,0,0) to C(0,0,10) ==> end: D (0,5,5)
  ==> No use case yet
