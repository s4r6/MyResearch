using System.Collections.Generic;
using Domain.Stage;
using NUnit.Framework;
using UnityEngine;
using View.UI;

public class TestResultView : MonoBehaviour
{
    [SerializeField]
    ResultView view;
    List<RiskAssessmentHistory> histories = new()
{
    // ����F�ő�50 �� -15 ������ 35
    new RiskAssessmentHistory(
        "PC01",
        "�p�X���[�h�\�t",
        "�p�X���[�h�𔍂�����",
        -15, 35, 50,
        3, 7, 10,
        "PC�{�̂ɓ\���Ă����p�X���[�h�𔍂������ƂŁA��O�҂ɂ��s�����O�C�����X�N��啝�Ɍy�������B"),

    // ���F35 �� -10 ������ 25
    new RiskAssessmentHistory(
        "�T�[�o�[���[����",
        "�J�����",
        "�{������",
        -10, 25, 50,
        2, 5, 10,
        "�N�ł�����������Ԃ������T�[�o�[���[�����{�����A�����I�ȕs���A�N�Z�X�̃��X�N��ቺ�������B"),

    // ���F25 �� -8 ������ 17
    new RiskAssessmentHistory(
        "USB������",
        "�����o�������Ȃ�",
        "������ĕۊǂ���",
        -8, 17, 50,
        1, 4, 10,
        "�����Ȃ������o����Ă���USB��������K�؂ɉ���E�ۊǂ��邱�ƂŁA���R�k�̃��X�N���y�������B"),

    // ���F17 �� -5 ������ 12
    new RiskAssessmentHistory(
        "����01",
        "�Ԗ{�����{��",
        "����������",
        -5, 12, 50,
        2, 2, 10,
        "�d�v���ނł���Ԗ{���ۊǂ��ꂽ���ɂ��{�����A�������̕s���擾���X�N�������������B"),

    // ���F12 �� -6 ������ 6�i�ŏI��ԁj
    new RiskAssessmentHistory(
        "�Ǘ�PC",
        "��ʃ��b�N�Ȃ�",
        "��ʃ��b�N��ݒ肵��",
        -6, 6, 50,
        1, 1, 10,
        "���h���������Ǘ�PC�ɉ�ʃ��b�N��ݒ肵�A�Ȃ𗣂ꂽ�ۂ̕s�����샊�X�N��}�����B")
};

    private void Start()
    {
        view.ShowResultWindow(histories);
    }
}
