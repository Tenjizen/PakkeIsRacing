using UnityEngine;

namespace UI.Menu
{
    public class NotificationsController : MonoBehaviour
    {
        [SerializeField] private Animator _questNotificationAnimator;
        [SerializeField] private Animator _skillPointNotificationAnimator;
        [SerializeField] private Animator _questCompletedNotificationAnimator;

        public void LaunchQuestNotification()
        {
            _questNotificationAnimator.SetTrigger("LaunchAnimation");
        }
        
        public void LaunchSkillPointNotification()
        {
            _skillPointNotificationAnimator.SetTrigger("LaunchAnimation");
        }
        
        public void LaunchQuestCompletedNotification()
        {
            _questCompletedNotificationAnimator.SetTrigger("LaunchAnimation");
        }
    }
}