using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Afterglow.M01
{
    /// <summary>
    /// Escolta (Q05): con el encargo activo, hablar con el minero perdido hace que siga al jugador por las puertas.
    /// Se entrega al llegar caminando a la escena de destino. T queda bloqueado durante la escolta (M01InventoryPhone).
    /// Morir o abandonar la expedición la cancela: el minero vuelve a su sitio y hay que retomarla desde él.
    /// </summary>
    public sealed class M01EscortNPC : MonoBehaviour
    {
        public M01Dialogue dialogue;
        public string questId = "Q05", deliverScene = "N03S01", deliverEvent = "Q05.ENTREGADO", homeScene;
        public string[] lineIds;
        public GameObject label;
        public static M01EscortNPC Follower { get; private set; }
        InputAction interact; bool requested; AfterglowPlayer player; Vector3 velocity;
        M01Progress Data => M01Session.Instance.data;

        void OnEnable() { interact = new InputAction("Escort", InputActionType.Button, "<Keyboard>/e"); interact.performed += _ => { if (dialogue && !dialogue.IsOpen) requested = true; }; interact.Enable(); SceneManager.sceneLoaded += Arrived; }
        void OnDisable() { interact?.Dispose(); SceneManager.sceneLoaded -= Arrived; }

        void Start()
        {
            if (string.IsNullOrEmpty(homeScene)) homeScene = gameObject.scene.name;
            if (Follower != this && (Data.events.Contains(deliverEvent) || (Data.escortActive && Follower))) { gameObject.SetActive(false); return; }
            player = FindAnyObjectByType<AfterglowPlayer>();
        }

        void Arrived(Scene scene, LoadSceneMode mode)
        {
            if (Follower != this) return;
            player = FindAnyObjectByType<AfterglowPlayer>(); dialogue = FindAnyObjectByType<M01Dialogue>();
            if (!Data.escortActive) { Destroy(gameObject); Follower = null; return; }
            if (scene.name == deliverScene)
            {
                Data.escortActive = false; if (!Data.events.Contains(deliverEvent)) Data.events.Add(deliverEvent);
                M01Toast.Message("El minero perdido llegó a la entrada", 2.6f);
                Destroy(gameObject); Follower = null;
            }
        }

        void Update()
        {
            if (!player) player = FindAnyObjectByType<AfterglowPlayer>();
            if (Follower == this)
            {
                if (!Data.escortActive || (player && player.Dead)) { Data.escortActive = false; Follower = null; Destroy(gameObject); return; }
                if (player && !M01Expedition.Transitioning)
                {
                    var target = player.transform.position - player.Facing * 1.3f;
                    if (Vector3.Distance(transform.position, player.transform.position) > 6) transform.position = target;
                    transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, .35f);
                }
                return;
            }
            if (!requested) return; requested = false;
            if (!player || player.Dead || dialogue.IsOpen || Vector3.Distance(player.transform.position, transform.position) > 2.2f) return;
            var quest = Data.Quest(questId);
            if (quest.paid) return;
            // SEC.09: el propio minero pide que lo guíes; aceptar activa Q05 y la escolta.
            dialogue.Begin(lineIds, () => dialogue.BeginChoice("¿Guiarlo hasta la entrada de las galerías?", new[] { "Aceptar", "Ahora no" }, choice =>
            {
                if (choice != 0) return;
                quest.active = true; quest.attempt = Data.expeditionAttempt;
                Data.escortActive = true; Follower = this; if (label) label.SetActive(false); transform.SetParent(null); DontDestroyOnLoad(gameObject);
                M01Toast.Message("El minero perdido te sigue", 2.2f);
            }, "Encargo " + questId));
        }
    }
}
