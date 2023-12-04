using System.Collections.Generic;

using jmayberry.GambitSystem;

public class AIContext : IGambitContext {
	public AICharacter Character { get; private set; }
	public List<AICharacter> Enemies { get; private set; }
	public AICharacter Target { get; set; }

	// Constructor
	public AIContext(AICharacter character, List<AICharacter> enemies) {
		Character = character;
		Enemies = enemies;
	}
}
