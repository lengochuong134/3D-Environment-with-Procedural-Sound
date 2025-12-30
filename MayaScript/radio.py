# Maya Python script to create a simple radio model with materials
import maya.cmds as cmds

# Build radio model
radio_body = cmds.polyCube(w = 6, h = 4, d = 1.5, n = 'radio_body')[0]
atten = cmds.polyCylinder(r=0.07, h= 5, sx=8, name='atten')[0]
cmds.move(0.5, 3, 0, atten)
cmds.rotate(0, 0, 60, atten)
button01 = cmds.polyCylinder(r=0.5, h=0.5, sx=16, name='button01')[0]
cmds.move(-1.7, 1.0, 0.8, button01)
cmds.rotate(90, 0, 0, button01)
button02 = cmds.polyCylinder(r=0.5, h=0.5, sx=16, name='button02')[0]
cmds.move(1.6, 1.0, 0.8, button02)
cmds.rotate(90, 0, 0, button02)
radio_detail = cmds.polyCube(w = 4.5, h = 2.0, d = 0.1, n = 'radio_detail')[0]
cmds.move(0, -0.7, 0.8, radio_detail)

# Assign materials
metal_shader = cmds.shadingNode('blinn', asShader=True, name='metalShader')
cmds.setAttr(metal_shader + ".color", 0.7, 0.6, 0.2, type="double3")
cmds.setAttr(metal_shader + ".specularColor", 1, 1, 1, type="double3")
cmds.select(radio_body)
cmds.hyperShade(assign=metal_shader)
cmds.select(atten)
cmds.hyperShade(assign=metal_shader)
wood_shader = cmds.shadingNode('lambert', asShader=True, name='woodShader')
cmds.setAttr(wood_shader + ".color", 0.4, 0.2, 0.1, type="double3")
cmds.select(button01)
cmds.hyperShade(assign=wood_shader)
cmds.select(button02)
cmds.hyperShade(assign=wood_shader)
cmds.select(radio_detail)
cmds.hyperShade(assign=wood_shader)



