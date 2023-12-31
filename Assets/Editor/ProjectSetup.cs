using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;






namespace Akthanar.EditorTool
{
    // --------------------
    using MAKE = GUILayout;
    using EGUI = EditorGUI;
    using STYLE = EditorStyles;
    using SETUP = GUILayoutOption;
    using FIELD = EditorGUILayout;
    using EGUIU = EditorGUIUtility;
    // ----------------------------
    
    

    public class ProjectSetupPanel : EditorWindow
    {
        const string MENU_PATH = "Tools/Setup Helper Panel";
        const string PANEL_NAME = "ProjectSetup";
        const string STORAGE_PATH = "Assets/Editor/storage.asset";

        private EditorScriptMemory load;

        void OnEnable () => load ??= AssetDatabase.LoadAssetAtPath<EditorScriptMemory>(STORAGE_PATH);

        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();


        #region CREATE_PANEL
        [MenuItem(MENU_PATH)]
        public static void ShowWindow() => GetWindow<ProjectSetupPanel>(PANEL_NAME);
        #endregion



        // void Update() { EditorApplication.update += RepaintEveryFrame; }

        // void RepaintEveryFrame() { Repaint(); }  // chiama questo dall'update
        // void OnInspectorUpdate() { Repaint(); }  // quando modifichi l'inspector
        // void OnHierarchyChange() { Repaint(); }  // quando modifichi la hierarchy
        // void OnSelectionChange() { Repaint(); }  // quando cambi oggetto selezionato


        // eseguito quando si muove il mouse sulla finestra
        // o quando viene chiamato il metodo Repaint()
        // ------------------------------------------------
        private void OnGUI()
        {
            MAKE.Space(20);
            load = (EditorScriptMemory)FIELD.ObjectField("Storage", load, typeof(EditorScriptMemory), false);
            MAKE.Space(20);

            MAKE.Label("Inventory", STYLE.boldLabel); MAKE.Space(10);

            load.inventory = (Inventory)FIELD.ObjectField("ref: Script", load.inventory, typeof(Inventory), true);            
            load.hotbar = (GameObject)FIELD.ObjectField("ref: Hotbar", load.hotbar, typeof(GameObject), true);
            
            var inv = load.inventory;


            MAKE.Space(20);

            if (MAKE.Button("Reload armor slot and weapons"))
            {
                var slots = load.inventory.GetComponentsInChildren<ArmorSlot>().ToList();

                // find element of list with slotType == mainHand
                inv.mainHandSlot = slots.Find(x => x.slotType == ArmorSlot.SlotType.mainHand);
                slots.Remove(inv.mainHandSlot);

                // find element of list with slotType == offHand
                inv.offHandSlot = slots.Find(x => x.slotType == ArmorSlot.SlotType.offHand);
                slots.Remove(inv.offHandSlot);

                inv.armorSlots = slots.ToArray();
            }

            if (MAKE.Button("Assign values to armor slots"))
            {
                foreach (var item in inv.armorSlots) SetSerializedValue(item);
                
                SetSerializedValue(inv.mainHandSlot);
                SetSerializedValue(inv.offHandSlot);
            }

            if (MAKE.Button("Make armos buttons not interactable"))
            {
                foreach (var item in inv.armorSlots) item.GetComponent<Button>().interactable = false;
                inv.mainHandSlot.GetComponent<Button>().interactable = false;
                inv.offHandSlot.GetComponent<Button>().interactable = false;
            }

        }

        private void SetSerializedValue(ArmorSlot value)
        {
            // serializza item
            SerializedObject serializedItem = new(value);

            serializedItem.Update();

            // serializza itemIcon e typeIcon
            SerializedProperty typeIcon = serializedItem.FindProperty("typeIcon");
            SerializedProperty itemIcon = serializedItem.FindProperty("itemIcon");

            typeIcon.objectReferenceValue = value.transform.GetChild(0).GetComponent<Image>();
            itemIcon.objectReferenceValue = value.transform.GetChild(1).GetComponent<Image>();

            serializedItem.ApplyModifiedProperties();
        }
    }
}