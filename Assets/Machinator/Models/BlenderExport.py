import bpy
import os

def remove_default_collection_with_objects():
    # The default collection is usually called "Collection"
    default_coll = bpy.data.collections.get("Collection")
    
    if default_coll:
        # Delete all objects inside the collection
        for obj in default_coll.objects:
            bpy.data.objects.remove(obj, do_unlink=True)
        
        # Delete the collection itself
        bpy.data.collections.remove(default_coll)
        print("Default collection and all its objects have been removed.")
    else:
        print("Default collection not found.")

# Call the function immediately when the script runs
remove_default_collection_with_objects()

def add_empty_parents_to_collections():
    scene_collection = bpy.context.scene.collection

    for coll in bpy.data.collections:
        if coll == scene_collection:
            continue  # Skip the main scene collection

        if len(coll.objects) == 0:
            continue  # Skip empty collections

        # Create an empty object named after the collection
        empty = bpy.data.objects.new(name=coll.name + "_Empty", object_data=None)
        bpy.context.scene.collection.objects.link(empty)

        # Make all objects in the collection children of the empty
        for obj in coll.objects:
            obj.parent = empty

# Run the function
add_empty_parents_to_collections()
print("Empty parents added for all collections.")

def sync_timeline_to_nla():
    max_frame = 1
    
    # 1. Check all Armatures and Objects for NLA tracks
    for obj in bpy.data.objects:
        if obj.animation_data and obj.animation_data.nla_tracks:
            for track in obj.animation_data.nla_tracks:
                for strip in track.strips:
                    # Check the end point of each strip
                    if strip.frame_end > max_frame:
                        max_frame = int(strip.frame_end)
        
        # 2. Also check if there is an active Action (not yet pushed to NLA)
        if obj.animation_data and obj.animation_data.action:
            action_end = obj.animation_data.action.frame_range[1]
            if action_end > max_frame:
                max_frame = int(action_end)

    # 3. Apply the found maximum frame to the scene timeline
    if max_frame > 1:
        bpy.context.scene.frame_end = max_frame
        print(f"Timeline End successfully set to: {max_frame}")
    else:
        # Fallback to a default value if no animation is found
        bpy.context.scene.frame_end = 64
        print("No animation found. Timeline End set to default: 32")

# Run the script
sync_timeline_to_nla()

def export_with_empties():
    folder = "C:\\EXport\\"
    if not os.path.exists(folder):
        os.makedirs(folder)
        
    path = os.path.join(folder, "unity_final_with_points.fbx")

    # 1. Make sure we are in Object Mode
    if bpy.context.active_object and bpy.context.active_object.mode != 'OBJECT':
        bpy.ops.object.mode_set(mode='OBJECT')

    # 2. Export including empty objects
    print("Exporting meshes, skeleton, and load points (empties)...")
    
    bpy.ops.export_scene.fbx(
        filepath=path,
        use_selection=False,
        # Include 'EMPTY' in object types
        object_types={'ARMATURE', 'MESH', 'EMPTY'},
        
        # Animation settings (export a single clean clip)
        bake_anim=True,
        bake_anim_use_all_bones=True,
        bake_anim_use_nla_strips=False,    
        bake_anim_use_all_actions=False,   
        bake_anim_force_startend_keying=False,
        
        # Optimization (reduce keyframes)
        bake_anim_simplify_factor=1.0, 
        
        # Axes and scale
        add_leaf_bones=False, 
        axis_forward='-Z',
        axis_up='Y',
        apply_scale_options='FBX_SCALE_ALL'
    )
    
    print(f"--- DONE! Check the file: {path} ---")

export_with_empties()