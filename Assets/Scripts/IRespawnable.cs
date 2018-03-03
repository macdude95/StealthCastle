/* IRespawnable.cs
 * Created by Michael Cantrell
 * Any class that implements this interface should define the "Respawn"
 * method that is automatically called by the GameController when the objects 
 * in the scene are to be reset
 */
public interface IRespawnable {
    void Respawn();
}
