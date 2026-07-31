# Unity Graph Toolkit

Standalone Unity 6 host project for a reusable graph foundation, NPBehave visual behavior trees, and a red-dot graph system.

## Packages

- `com.alelievr.node-graph-processor`: upstream NodeGraphProcessor, pinned from Git through Unity Package Manager.
- `com.graphtoolkit.foundation`: PiRho Utilities and deterministic node auto-layout.
- `com.graphtoolkit.npbehave`: behavior-tree runtime, visual editor and safe JSON serialization.
- `com.graphtoolkit.reddot`: red-dot runtime, visual editor, safe JSON export and generated executors.

The project intentionally contains no game-specific runtime layers, production graphs, or source-project asset paths.

`com.unity.pipeline@0.4.0-exp.1` is installed from Unity's registry by the official Unity CLI and is not vendored or modified.
