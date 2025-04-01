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
    private int currentTicketPool;
    private float ticketTimer;
    private float givingTicketTimer;
    private List<Enemy> enemiesReadyToAttack = new List<Enemy>(); //Enemies that are ready to use their attack state

    //Enemies in combat list and associated methods to access/add data
    private Dictionary<Enemy, bool> enemiesInCombat = new Dictionary<Enemy, bool>(); //Enemies that have the player in their vision range
    public Dictionary<Enemy, bool> EnemiesInCombat { get { return enemiesInCombat; } }

    private int CurrentTicketTotal
    { 
        get 
            {
                int count = 0;
                foreach(KeyValuePair<Enemy, bool> keyValuePair in enemiesInCombat)
                {
                    if(keyValuePair.Key)
                        count++;
                }
                return currentTicketPool + count; 
            } 
    }

    public void AddEnemyToCombatList(Enemy enemy)
    {
        if(!enemiesInCombat.ContainsKey(enemy))
            enemiesInCombat.Add(enemy, false);
    }

    public void RemoveEnemyFromCombatList(Enemy enemy)
    {
        if (!enemiesInCombat.ContainsKey(enemy))
            return;

        if (enemiesInCombat[enemy])
        {
            currentTicketPool--;
        }
        enemiesInCombat.Remove(enemy);
        ReleaseAttack(enemy);
    }

    public bool IsWaitingForAttack(Enemy enemy)
    {
        return enemiesInCombat.ContainsKey(enemy);
    }

    public void UseAttack(Enemy enemy)
    {
        if (!enemiesInCombat.ContainsKey(enemy))
            return;

        enemiesInCombat[enemy] = false;
    }
    #endregion

    //Runtime Methods ---------------------------------------------------------------------------------------------
    #region Runtime
    // Start is called before the first frame update
    void Start()
    {
        currentTicketPool = 0;
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
            if(CurrentTicketTotal < maxTicketTotal)
            {
                currentTicketPool++;
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

    //Gives attacks to enemies if conditions are met and handles associated variables
    public void GiveAttack()
    {
        givingTicketTimer += Time.deltaTime;

        if (currentTicketPool > 0 && givingTicketTimer > timerBetweenGivingTicket && enemiesReadyToAttack.Count > 0)
        {
            enemiesReadyToAttack[0].HasAttackTicket = true;
            enemiesReadyToAttack.RemoveAt(0);
            givingTicketTimer = 0;
        }   
    }
    #endregion

}
