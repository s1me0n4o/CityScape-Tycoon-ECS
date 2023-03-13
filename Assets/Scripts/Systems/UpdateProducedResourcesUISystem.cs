using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(LateSimulationSystemGroup))] // to run at the end of the frame
public partial class UpdateProducedResourcesUISystem : SystemBase
{
    private static List<TextMeshPro> _textMeshPros = new List<TextMeshPro>();
    protected override void OnStartRunning()
    {
        Entities
        .WithAll<ProducerTag>()
        .ForEach((GeneratedResourcesUIData uiData, in Translation translation, in ProducerData producerData) =>
        {
            var gameObject = new GameObject("ProducerText", typeof(TextMeshPro));
            var transform = gameObject.transform;
            transform.localPosition = translation.Value + uiData.PositionOffset;
            var tmp = gameObject.GetComponent<TextMeshPro>();
            tmp.rectTransform.sizeDelta = new Vector2(1, 1);

            tmp.alignment = TextAlignmentOptions.Center;
            tmp.text = producerData.CurrentAmount.ToString();
            tmp.fontSize = 8;
            tmp.color = Color.black;
            tmp.GetComponent<MeshRenderer>().sortingOrder = 0;
            _textMeshPros.Add(tmp);

        }).WithoutBurst().Run();
    }

    protected override void OnUpdate()
    {
        Entities
        .WithAll<ProducerTag>()
        .ForEach((in ProducerData producerData) =>
        {
            if (!_textMeshPros.Any())
                return;
            foreach (var item in _textMeshPros)
            {
                item.SetText(producerData.CurrentAmount.ToString());
            }
        }).WithoutBurst().Run();
    }
}
