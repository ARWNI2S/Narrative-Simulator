# Narrative Interactive Intelligent Simulator
Automated Game Mastering for Open World MMORPG

![This is an image](/../../../../ARWNI2S/.github/blob/main/assets/images/work-in-progress.png)

## NI2S main repository. 
With the code in this repository, you can build the full Narrative Interactive Intelligent Simulator (NI2S) system for Windows (, and Linux, coming soon...); compile NI2S based game services for a variety of target engines, including Unreal Engine, Unity(coming soon...), and Cry Engine(coming soon...); and build tools like NI2S Knowledge and NI2S Frontline. Modify the code in any way you can imagine, and share your changes with others!

We are gathering any [available documentation](https://github.com/ARWNI2S/NI2S-Documentation) for the entire development process of NI2S. If you're looking for the answer to something, you may want to start in one of these places:

*   Read the introductory [NI2S Whitepaper](https://github.com/ARWNI2S/NI2S-Documentation/blob/main/pub/Whitepaper.pdf).
*   [Creating Narrative Worlds with NI2S](https://github.com/ARWNI2S/NI2S-Documentation/wiki/Creating-narrative-worlds-with-NI2S/).
*   [Development Setup](https://github.com/ARWNI2S/NI2S-Documentation/wiki/Setting-up-your-development-environment-for-NI2S/).
*   [Building the GitHub source code](https://github.com/ARWNI2S/NI2S-Documentation/wiki/Building-NI2S-source-code/).
*   [NI2S API Reference](https://github.com/ARWNI2S/NI2S-Documentation/blob/main/XMLHelp/)

If you need more, just ask! Many NI2S developers read the [Discussion](https://not.available.yet/latest?exclude_tag=question) and [Q&A](https://not.available.yet/tag/question) forums on the [NI2S Dev Community](https://not.available.yet/community/) site.

## NI2S runtime.
The NI2S runtime is what implements the distributed simulation model. By design, NI2S requires an implementation of the distributed actor model, so we are using a modified version of [dotnet/Orleans](https://github.com/dotnet/orleans) specifically designed for narrative simulation.

The main component of the NI2S runtime is the node, which is hosting simulable entities. A group of NI2S nodes should be running as a cluster, for scalability, fault-tolerance, workload distribution and simulation syncronization. The runtime enables entities hosted in the cluster to communicate with each other as if they are within a single, but time-sorted, event queue.

From a more technical point of wiew the changes made to Orleans are related to implement GDESK in the message queue, and composition to distributed actor model. 

In addition to the distributed simulation model, the node engine provides a set of entities with predefined framework services, such as narrators, persistence, transactions, streams, and more. See the [features section](https://github.com/ARWNI2S/NI2S-Infrastructure#features) for more detail.

Frontend server clients call entities in the cluster using the client library which automatically manages network communication. Clients can also be co-hosted in the same process with silos for simplicity.

Orleans is compatible with .NET Standard 2.0 and above, running on Windows, Linux, and macOS, in full .NET Framework or .NET Core.

//TODO: Rewrite over time...