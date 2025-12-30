# Maya Python script to create a bell model with materials
import maya.cmds as cmds

# Build bell model
bowl = cmds.polySphere(r=5, sx=40, sy=40, name='bowl')[0]
cmds.scale(1.3, 1.2, 1.3, bowl)
cmds.move(0, 6.5, 0, bowl)

cmds.select('bowl.f[800:1599]')  
cmds.delete()

cmds.select('bowl.f[0:799]')
cmds.polyExtrudeFacet(localTranslateZ=-0.3)  
cmds.polyExtrudeFacet(localTranslateZ=0.5)

base = cmds.polyCylinder(r=4, h=0.3, sx=40, name='base')[0]
cmds.move(0, 0.15, 0, base)

cmds.select(bowl, base)
cmds.polyUnite(name='bowl_final')
cmds.delete(ch=True)

stick = cmds.polyCylinder(r=0.5, h=10, sx=40, name='stick')[0]
cmds.move(7, 0, 0, stick)
cmds.rotate(90, 0, 90, stick)
cmds.polyBevel3(offset=0.05, segments=3) 

# Assign materials
metal_shader = cmds.shadingNode('blinn', asShader=True, name='metalShader')
cmds.setAttr(metal_shader + ".color", 0.7, 0.6, 0.2, type="double3")
cmds.setAttr(metal_shader + ".specularColor", 1, 1, 1, type="double3")
cmds.select('bowl_final')
cmds.hyperShade(assign=metal_shader)

wood_shader = cmds.shadingNode('lambert', asShader=True, name='woodShader')
cmds.setAttr(wood_shader + ".color", 0.4, 0.2, 0.1, type="double3")
cmds.select('stick')
cmds.hyperShade(assign=wood_shader)