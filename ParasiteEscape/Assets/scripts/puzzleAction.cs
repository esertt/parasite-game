using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class PuzzleAction : MonoBehaviour
{
    [Header("Parasite Setup")]
    public GameObject parasite;
    public Transform movePoint;
    public float speed = 5f;
    private SpriteRenderer sr;
    private string isInfected = "parasite";
    private Sprite originalParasiteSprite;
    public AudioSource babyDie, scientistDie, gunShot, parasitePossess, buttonClick;
    public AudioSource manHitWall, parasiteHitWall, parasiteWalk, humanStep, doorOpen;
    // small timers to avoid spamming walk/step sounds
    private Vector3 lastParasitePos;
    private float parasiteWalkTimer = 0f;
    // make footsteps less spammy by increasing the interval and movement threshold
    private float parasiteWalkInterval = 0.10f; // seconds between footstep sounds
    private float humanStepTimer = 0f;
    private float humanStepInterval = 0.2f; // rate-limit human step SFX when possessing a scientist

    [Header("Tilemap & NPCs")]
    public Tilemap tileMap;
    public int targetLayer;
    private List<GameObject> objectsInLayer = new List<GameObject>();

    [Header("NPC Movement")]
    public bool NPCREQUEM = false;

    private bool actionReady = true;
    private Animator anim;

    private void Start()
    {
        isInfected = "parasite";
        movePoint.parent = null;
        sr = parasite.GetComponent<SpriteRenderer>();

        // Gather all objects in the target layer
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.layer == targetLayer)
            {
                objectsInLayer.Add(obj);
                Debug.Log("Found object: " + obj.name);
            }
        }

        sr = parasite.GetComponent<SpriteRenderer>();
        // save original parasite sprite so we can restore it on release
        if (sr != null) originalParasiteSprite = sr.sprite;
        if (parasite != null) lastParasitePos = parasite.transform.position;
        anim = parasite.GetComponent<Animator>();
    }

    private void Update()
    {
        MoveParasite();
        // update timers
        if (parasiteWalkTimer > 0f) parasiteWalkTimer -= Time.deltaTime;
        if (humanStepTimer > 0f) humanStepTimer -= Time.deltaTime;
        // If currently possessing someone, allow player to sacrifice them with Space
        if (possessedNPC != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                KillPossessed();
            }
        }
        if (actionReady)
        {
            HandleInputMovement();
            CheckForPossession();
            CheckForSoldierThreat();
            if (NPCREQUEM) npcAction();
        }
    }

    private void MoveParasite()
    {
        parasite.transform.position = Vector3.MoveTowards(parasite.transform.position, movePoint.position, speed * Time.deltaTime);
        actionReady = Vector3.Distance(parasite.transform.position, movePoint.position) <= 0f;

        // play footstep sound while parasite moves
        if (sr != null && parasite != null)
        {
            float moved = Vector3.Distance(parasite.transform.position, lastParasitePos);
            // require a slightly larger movement delta to trigger footsteps
            if (moved > 0.01f)
            {
                // timer-driven footsteps: play a one-shot footstep clip at intervals while moving
                if (parasiteWalkTimer <= 0f)
                {
                    string infectedLower = isInfected != null ? isInfected.ToLower() : "";
                    if ((infectedLower == "yellowscientist" || infectedLower == "bluescientist") && humanStep != null)
                    {
                        if (humanStep.clip != null)
                            humanStep.PlayOneShot(humanStep.clip);
                        else
                            humanStep.Play();
                    }
                    else
                    {
                        if (parasiteWalk != null && infectedLower == "parasite")
                        {
                            if (parasiteWalk.clip != null)
                                parasiteWalk.PlayOneShot(parasiteWalk.clip);
                            else
                                parasiteWalk.Play();
                        }
                    }
                    parasiteWalkTimer = parasiteWalkInterval;
                }
            }
            else
            {
                // stationary: ensure no loop flags are left on audio sources and stop them if they were looping
                if (humanStep != null && humanStep.isPlaying && humanStep.loop)
                {
                    humanStep.Stop();
                    humanStep.loop = false;
                }
                if (parasiteWalk != null && parasiteWalk.isPlaying && parasiteWalk.loop)
                {
                    parasiteWalk.Stop();
                    parasiteWalk.loop = false;
                }
            }
            lastParasitePos = parasite.transform.position;
        }
    }

    private void HandleInputMovement()
    {
        Vector3 newPos = movePoint.position;
        bool moveNPC = false;
        bool attemptedMoveIntoWall = false;

        // Horizontal input
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) == 1)
        {
            newPos += new Vector3(h, 0f, 0f);
            moveNPC = true;
        }

        // Vertical input
        float v = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(v) == 1)
        {
            newPos += new Vector3(0f, v, 0f);
            moveNPC = true;
        }

        // Check if target tile exists and whether it's passable
        Vector3Int cellPos = tileMap.WorldToCell(newPos);
        TileBase tile = tileMap.GetTile(cellPos);
        bool passable = true;
        if (tile == null)
        {
            passable = false; // no tile -> treat as wall/border
        }
        else
        {
            // You can augment this check by tile name or by setting a Tile flag/property
            string tname = tile.name != null ? tile.name : "";
            // if (!(tname.Contains("tiletest_FINAL_REAL_45") || tname.Contains("tiletest_FINAL_REAL_46") ||
            //     tname.Contains("tiletest_FINAL_REAL_47") || tname.Contains("tiletest_FINAL_REAL_48") ||
            //     tname.Contains("tiletest_FINAL_REAL_49") ) ) passable = false;
            // TODO: UNCOMMENT WHEN CREATED ACTUAL LEVELS
        }

        if (!passable)
        {
            attemptedMoveIntoWall = true;
            // play wall-hit sound (rate-limited)
            // if currently possessing a scientist/human, play manHitWall; otherwise parasiteHitWall
            string infectedLower = isInfected != null ? isInfected.ToLower() : "";
            if (attemptedMoveIntoWall)
            {
                // wall hit cooldown handled by parasiteWalkTimer to keep it simple
                if (parasiteWalkTimer <= 0f)
                {
                    if ((infectedLower == "yellowscientist" || infectedLower == "bluescientist") && manHitWall != null)
                    {
                        if (manHitWall.clip != null) manHitWall.PlayOneShot(manHitWall.clip);
                        else manHitWall.Play();
                    }
                    else if (parasiteHitWall != null)
                    {
                        if (parasiteHitWall.clip != null) parasiteHitWall.PlayOneShot(parasiteHitWall.clip);
                        else parasiteHitWall.Play();
                    }
                    parasiteWalkTimer = 0.25f; // short cooldown for wall hit sound
                }
            }
        }
        else
        {
            // Before moving, check for laser probes blocking the tile
            if (IsProbeAtCell(cellPos))
            {
                // play wall-hit sound appropriate to current form
                string infectedLower = isInfected != null ? isInfected.ToLower() : "";
                if ((infectedLower == "yellowscientist" || infectedLower == "bluescientist") && manHitWall != null)
                {
                    if (manHitWall.clip != null) manHitWall.PlayOneShot(manHitWall.clip);
                    else manHitWall.Play();
                }
                else if (parasiteHitWall != null)
                {
                    if (parasiteHitWall.clip != null) parasiteHitWall.PlayOneShot(parasiteHitWall.clip);
                    else parasiteHitWall.Play();
                }
                // don't move into the probe
                return;
            }

            // Move parasite
            movePoint.position = newPos;
            if (moveNPC) npcAction();

            // After moving, check if we've stepped on the laser ray
            CheckLaserAtCell(cellPos);
        }
    }

    // Returns true if any Laser probe occupies the given tile cell
    private bool IsProbeAtCell(Vector3Int cellPos)
    {
        Lasers[] lasers = UnityEngine.Object.FindObjectsByType<Lasers>(FindObjectsSortMode.None);
        foreach (Lasers l in lasers)
        {
            if (l == null) continue;
            if (!l.isOn) continue;
            if (l.LaserProbe1 != null && l.LaserProbe1.activeInHierarchy)
            {
                if (tileMap.WorldToCell(l.LaserProbe1.transform.position) == cellPos) return true;
            }
            if (l.LaserProbe2 != null && l.LaserProbe2.activeInHierarchy)
            {
                if (tileMap.WorldToCell(l.LaserProbe2.transform.position) == cellPos) return true;
            }
        }
        return false;
    }

    // Checks if a laser ray exists at the given tile cell. If so, kill the parasite instantly.
    private void CheckLaserAtCell(Vector3Int cellPos)
    {
        Lasers[] lasers = UnityEngine.Object.FindObjectsByType<Lasers>(FindObjectsSortMode.None);
        foreach (Lasers l in lasers)
        {
            if (l == null) continue;
            if (!l.isOn) continue;
            if (l.LaserRay != null && l.LaserRay.activeInHierarchy)
            {
                if (tileMap.WorldToCell(l.LaserRay.transform.position) == cellPos)
                {
                    Debug.Log("Parasite hit laser ray at " + cellPos + " - dying.");
                    if (parasiteHitWall != null)
                    {
                        if (parasiteHitWall.clip != null) parasiteHitWall.PlayOneShot(parasiteHitWall.clip);
                        else parasiteHitWall.Play();
                    }
                    // If currently possessing someone, destroy them first
                    if (possessedNPC != null)
                    {
                        KillPossessed();
                    }
                    DieAndLoadGameOver();
                    return;
                }
            }
        }
    }

    private GameObject possessedNPC;
    private void CheckForPossession()
    {
        for (int i = objectsInLayer.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsInLayer[i];
            if (obj == null) continue;
            if (obj == parasite) continue;

            float distance = Vector3.Distance(parasite.transform.position, obj.transform.position);

            if (distance <= 1f && isInfected == "parasite") // threshold to possess
            {
                // Get the NPC sprite renderer (support sprites on children)
                SpriteRenderer npcSpriteRenderer = obj.GetComponentInChildren<SpriteRenderer>();
                Sprite npcSprite = npcSpriteRenderer != null ? npcSpriteRenderer.sprite : null;

                // normalize tag and handle different capitalizations
                string tagLower = obj.tag != null ? obj.tag.ToLower() : "";

                switch (tagLower)
                {
                    case "yellowscientist":
                        anim.SetTrigger("ToYellowScientist");
                        break;
                    case "bluescientist":
                        anim.SetTrigger("ToBlueScientist");
                        break;
                    case "baby":
                        anim.SetTrigger("ToBaby");
                        break;
                    default:
                        continue; // not possessable
                }

                // play possession sfx
                if (parasitePossess != null)
                {
                    if (parasitePossess.clip != null) parasitePossess.PlayOneShot(parasitePossess.clip);
                    else parasitePossess.Play();
                }

                // Update parasite internal state and visuals
                isInfected = tagLower;
                if (npcSprite != null && sr != null)
                {
                    sr.sprite = npcSprite; // copy visual from NPC
                    Debug.Log($"Possession: applied sprite from {obj.name} to parasite.");
                }
                else
                {
                    Debug.Log($"Possession: no sprite found on {obj.name}; parasite sprite unchanged.");
                }

                // preserve original NPC tag casing on the parasite GameObject
                parasite.tag = obj.tag;

                // Hide the NPC's sprite renderer (if present)
                if (npcSpriteRenderer != null)
                {
                    npcSpriteRenderer.enabled = false;
                }
                else
                {
                    // As a fallback, try disabling the root SpriteRenderer if present
                    SpriteRenderer rootSr = obj.GetComponent<SpriteRenderer>();
                    if (rootSr != null) rootSr.enabled = false;
                }

                possessedNPC = obj;
                Debug.Log("Possessed " + obj.tag);
                // small step sound for successful possession only if it's a scientist
                if ((tagLower == "yellowscientist" || tagLower == "bluescientist") && humanStep != null && humanStepTimer <= 0f)
                {
                    if (humanStep.clip != null) humanStep.PlayOneShot(humanStep.clip);
                    else humanStep.Play();
                    humanStepTimer = humanStepInterval;
                }
                break; // Possess only one at a time
            }
        }
    }

    private void ReleasePossession()
    {
        if (possessedNPC != null)
        {
            SpriteRenderer npcSpriteRenderer = possessedNPC.GetComponentInChildren<SpriteRenderer>();
            if (npcSpriteRenderer != null)
                npcSpriteRenderer.enabled = true;
            else
            {
                SpriteRenderer rootSr = possessedNPC.GetComponent<SpriteRenderer>();
                if (rootSr != null) rootSr.enabled = true;
            }
        }

        possessedNPC = null;
        isInfected = "parasite";
        parasite.tag = "parasite";
        // restore original parasite sprite if we saved it
        if (sr != null && originalParasiteSprite != null)
            sr.sprite = originalParasiteSprite;
        anim.SetTrigger("ToParasite"); // back to default animation
    }

    // Destroys the currently possessed NPC immediately and restores parasite state.
    // The destroyed NPC is removed from the objectsInLayer list so it won't respawn or be processed.
    private void KillPossessed()
    {
        if (possessedNPC == null) return;

        Debug.Log("KillPossessed: destroying " + possessedNPC.name + " (tag=" + possessedNPC.tag + ")");

        // play appropriate death sfx before destroying
        string tagLower = possessedNPC.tag != null ? possessedNPC.tag.ToLower() : "";
        if (tagLower == "baby")
        {
            if (babyDie != null)
            {
                if (babyDie.clip != null) babyDie.PlayOneShot(babyDie.clip);
                else babyDie.Play();
            }
        }
        else
        {
            // treat other possessed as scientist deaths
            if (scientistDie != null)
            {
                if (scientistDie.clip != null) scientistDie.PlayOneShot(scientistDie.clip);
                else scientistDie.Play();
            }
        }

        // Remove from list to avoid further interactions
        if (objectsInLayer.Contains(possessedNPC))
            objectsInLayer.Remove(possessedNPC);

        // Destroy the GameObject (no respawn)
        Destroy(possessedNPC);

        // Clear possession state and restore parasite visuals/state
        possessedNPC = null;
        isInfected = "parasite";
        parasite.tag = "parasite";
        if (sr != null && originalParasiteSprite != null)
            sr.sprite = originalParasiteSprite;
        anim.SetTrigger("ToParasite");
    }

    private void CheckForSoldierThreat()
    {
        foreach (GameObject obj in objectsInLayer)
        {
            if (obj == null) continue;
            string objTagLower = obj.tag != null ? obj.tag.ToLower() : "";
            if (objTagLower == "soldier")
            {
                float distance = Vector3.Distance(parasite.transform.position, obj.transform.position);
                if (distance <= 5f) // threat range
                {
                    // don't kill parasite if it's currently possessing a baby
                    string infectedLower = isInfected != null ? isInfected.ToLower() : "";
                    if (infectedLower == "baby")
                    {
                        continue;
                    }

                    Debug.Log(infectedLower + " killed by soldier!");
                    // play gunshot SFX if available
                        if (gunShot != null)
                        {
                            if (gunShot.clip != null) gunShot.PlayOneShot(gunShot.clip);
                            else gunShot.Play();
                        }
                    DieAndLoadGameOver();
                    // TODO: Game over scene trigger
                    return;
                }
            }
        }
    }

    private void npcAction()
    {
        // NPC actions no longer trigger a global step SFX here.
        foreach (GameObject obj in objectsInLayer)
        {
            NPCLogic npc = obj.GetComponent<NPCLogic>();
            if (npc != null)
            {
                npc.Action();
            }
        }
    }

    // Centralized death handler: load the GameOver scene when parasite dies
    private void DieAndLoadGameOver()
    {
        Debug.Log("Parasite died — loading GameOver scene.");
        // Optionally play a death SFX here before loading
        // Ensure the scene name matches your build settings
        SceneManager.LoadScene("GameOver");
    }
}