using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCoordinator : MonoBehaviour
{
    //Data & Variables --------------------------------------------------------------------------------------------
    #region Data & Variables
    //Variables to balance how tickets and associated values
    [SerializeField] private int maxTicketTotal = 2;
    [SerializeField] private float timerBetweenNewTicket = 1;
    [SerializeField] private float timerBetweenGivingTicket = .5f;

    //Backend variables to track current statuses
    private int currentTickets;
    private float ticketTimer;
    private float givingTicketTimer;
    private List<Enemy> enemiesReadyToAttack = new List<Enemy>(); 

    //Enemies in combat list and associated methods to access/add data
    private List<Enemy> enemiesInCombat = new List<Enemy>();
    public List<Enemy> EnemiesInCombat { get { return enemiesInCombat; } }

    public void AddEnemyToCombatList(Enemy enemy)
    {
        enemiesInCombat.Add(enemy);
    }
    #endregion

    //Runtime Methods ---------------------------------------------------------------------------------------------
    #region Runtime
    // Start is called before the first frame update
    void Start()
    {
        currentTickets = 0;
        ticketTimer = 0;
        givingTicketTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTickets();
        GiveAttack();
    }
    #endregion

    //Helper Methods ----------------------------------------------------------------------------------------------
    #region Helper Methods
    //Updates how many tickets there currently are
    private void UpdateTickets()
    {
        //Timer
        ticketTimer += Time.deltaTime;

        //Checks timer and max tickets, gives new ticket if conditions are met
        if(ticketTimer > timerBetweenNewTicket)
        {
            if(currentTickets <= maxTicketTotal)
            {
                currentTickets++;
            }
        }
    }

    //Public method where enemy can request to be added to the attack list
    public void RequestAttack(Enemy enemy)
    {
        enemiesReadyToAttack.Add(enemy);
    }

    //Public method where enemy requests to be removed from the queue
    public void ReleaseAttack(Enemy enemy)
    {
        enemiesReadyToAttack.Remove(enemy);
    }

    //Gives attacks to enemies if conditions are met and handles associateds variables
    public void GiveAttack()
    {
        givingTicketTimer += Time.deltaTime;

        if (currentTickets > 0 && givingTicketTimer > timerBetweenGivingTicket)
        {
            enemiesReadyToAttack[0].HasAttackTicket = true;
            enemiesReadyToAttack.RemoveAt(0);
            givingTicketTimer = 0;
            currentTickets--;
        }
    
    }
    #endregion

}
