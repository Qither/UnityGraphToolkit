# Unity Graph Toolkit

Standalone Unity 6 host project for a reusable graph foundation, NPBehave visual behavior trees, and a red-dot graph system.

## Packages

- `com.tdtoolkit.graph-foundation`: NodeGraphProcessor, PiRho Utilities and deterministic node auto-layout.
- `com.tdtoolkit.npbehave`: behavior-tree runtime, visual editor and safe JSON serialization.
- `com.tdtoolkit.reddot`: red-dot runtime, visual editor, safe JSON export and generated executors.

The project intentionally contains no game-specific runtime layers, production graphs, or source-project asset paths.

`com.unity.pipeline@0.4.0-exp.1` is installed from Unity's registry by the official Unity CLI and is not vendored or modified.
