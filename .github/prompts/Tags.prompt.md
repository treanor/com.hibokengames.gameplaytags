/*
This Unity package, `com.hibokengames.gameplaytags`, is a professional-quality, reusable gameplay tag system inspired by Unreal Engine's Gameplay Tags, designed for Unity 6.0.

Goal:
Build a lightweight, efficient, and modular gameplay tag system for use in any Unity project. While initially a personal tool, the package is designed with asset store release in mind — clean architecture, test coverage, documentation, and editor tooling.

Core Features:
- A runtime-only `TagManager` implemented as a singleton MonoBehaviour with `DontDestroyOnLoad`.
- A simple `Tag` class representing a single tag (e.g., "Enemy.Melee.Aggressive") as a string, with potential for future hierarchy support.
- Tag registration and lookup at runtime via methods like `RegisterTag`, `HasTag`, `GetTag`, and `GetAllTags`.
- Full Unity package structure (`Runtime/`, `Editor/`, `Tests/`, `.asmdef`, `package.json`)
- Clean, testable C# code using namespaces (`HibokenGames.GameplayTags`) and SOLID principles.
- Embedded runtime unit tests using Unity Test Framework under `Tests/Runtime`.
- Editor-only tooling (optional): tag viewer, validator, or future dropdown integration.

Target Behaviors:
- Tags should be fast to register and query.
- Re-registering a tag should be a no-op (no duplicates).
- System should be extensible for future features like tag containers or query matching (e.g., MatchesAny, MatchesAll).
- Tag system should work independently, and be consumable by other systems like ability systems, AI, or inventory.

Copilot should generate clean, modular code with XML docs, testable logic, and clear intent — building toward asset-store level quality.
*/
