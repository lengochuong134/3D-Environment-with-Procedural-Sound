# Maya Python script to create a wooden fish drum model with materials
import maya.cmds as cmds

# Build wooden fish drum model
fish_body = cmds.polySphere(r=4, sx=20, sy=20, name='fish_body')[0]
cmds.scale(1.6, 1.5, 1.9, fish_body)

stick = cmds.polyCylinder(r=0.5, h=15, sx=16, name='stick')[0]
cmds.rotate(0, 0, 90, stick)
cmds.move(0, -0.3, -3, stick)
cmds.scale(1.7, 2.0, 1.7, stick)
stick_sphere = cmds.polySphere(n = 'stick_sphere', r=2.0, sx = 16, sy = 16)[0]
cmds.move(-15, 0, -3, stick_sphere)

stick_boolean = cmds.polyCylinder(r=0.7, h=12, sx=16, name='stick_boolean')[0]
cmds.rotate(0, 0, 90, stick_boolean)
cmds.move(0, 0, -3, stick_boolean)
cmds.scale(1.7, 2.0, 1.7, stick_boolean)

fish_body = cmds.polyBoolOp(fish_body, stick_boolean, op=2, name='wooden_fish_drum')[0]

cube_boolean = cmds.polyCube(w=15, h=0.5, d=5, name='cube_boolean')[0]
cmds.move(0, 0, -6, cube_boolean)

fish_body = cmds.polyBoolOp(fish_body, cube_boolean, op=2, name='wooden_fish_drum')[0]

cmds.select(fish_body, stick_boolean, cube_boolean)
cmds.delete(ch = 1)

stage = cmds.polyCylinder(r=6, h=1, sx=20, name='stage')[0]
cmds.move(0, -6, 0, stage)
# Assign materials
metal_shader = cmds.shadingNode('blinn', asShader=True, name='metalShader')
cmds.setAttr(metal_shader + ".color", 0.7, 0.6, 0.2, type="double3")
cmds.setAttr(metal_shader + ".specularColor", 1, 1, 1, type="double3")
cmds.select(fish_body)
cmds.hyperShade(assign=metal_shader)
cmds.select(stage)
cmds.hyperShade(assign=metal_shader)

wood_shader = cmds.shadingNode('lambert', asShader=True, name='woodShader')
cmds.setAttr(wood_shader + ".color", 0.4, 0.2, 0.1, type="double3")
cmds.select(stick)
cmds.hyperShade(assign=wood_shader)

cmds.select(stick_sphere)
cmds.hyperShade(assign=wood_shader)