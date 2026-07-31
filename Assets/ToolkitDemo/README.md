# UnityGraphToolkit Demo

Use `Tools > Unity Graph Toolkit > Create or Rebuild Samples` once after importing the project.

- **Behavior Tree** opens a `Root -> Sequence -> Demo Set Blackboard` graph, exports it as JSON, then loads that TextAsset at runtime.
- **Red Dot** opens a normal-node -> ruled leaf -> multi-node graph. Generated partial methods drive two dynamic child values and a system-availability gate.
- **Demo > Run** opens the scene and enters Play Mode. The IMGUI buttons execute the behavior tree, change red-dot counts, and toggle the system gate.

The graph assets and generated JSON are small standalone examples only; no production data is included.
