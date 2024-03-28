# BoatGame

Hi there :)

I like boats. I wanted to make a game that revolves around some basic boat physics. *How hard can it be?*

I will be updating this whenever I add anything on project.

# 23/03/2024 & 24/03/2024
## Getting started

I started the project by setting up this GitHub repo and getting the .gitignore to a *somewhat* functional state. I found that I would have to make a few changes to it throught the early stages of the project.

The next part of the project was ~~planning carefully and doing weeks of research~~ making some waves.
After some research, I found that I could manually manipulate the vertecies of a mesh, constantly updating its mesh filter and collisions

### Manual Vertex Manipulation
	
Pros:
- This would make calculating waves very straight forward as i would be able to use the mesh collider to calculate waves using raycasts

Cons:
- It's the first thing I had thought of and therefore 99% the wrong method
- It's **incredibly** expensive to calculate all of the vertex points of a plane every Update / FixedUpdate on the CPU let alone the raycasts I would want to use to calculate buoyancy
- It's not a very scalable option should I want to create large surfaces of water

### Shaders

Pros:
- I can manipulate vertex's on the GPU with fantastic performance when compared to CPU calculations
- Scales well due to GPU rendering

Cons:
- It's ~~impossible~~ difficult to read data from the GPU onto the CPU unless you do Asynchronous voodoo magic™
- There is a lag period between when the asynchronous call starts and return results from the GPU to the CPU, resulting in inaccurate, out of sync calculations

## Writing Shader Code
I decided to go with the shader code approach to the waves as it would require less optimisation to scale up and it would be a cool thing to learn 

***Fun Fact:** I quickly figured out that shader code is not well supported in most IDEs, making it hell on earth to try and write code without having tons of code examples and documentation open and on screen all at once*

After reading into how games mimic waves and following a few tutorials, I was able to come up with a simple wave shader that manipulates the vertices of a plane to follow a simple Sin wave where I could manipulate the Amplitude and Wavelength of a wave. Though this was a great success, through my earlier research I knew of a better looking method to mimic waves. **Gerstner Waves** also known as **Trochoidal Waves** are an generally accepted method of calculating waves for computer simulations. They feature sharper peaks and wide troughs making them better suited to this role than simple Sin waves. Gerstner waves are best visualised by a rolling circle along a plane and drawing a line at a point of the circle closer to the center than its own radius.

This does come with the added side effect of having the vertices no longer being manipulated only along the Y-Axis, but also along the path of the wave.

# 27/03/2024 & 28/03/2024
## Getting the height of a Gerster Wave at a point

The key thing that I had to figure out with the shader approach is to make sure that I can calculate the height of a wave at a certain point. I cannot simply read the vertex positions of the shader because the calculations are on the GPU. I would have to request the vertex positions asynchronously which has the downside of having a couple ms delay, making the calculations inconsistent and inaccurate.
