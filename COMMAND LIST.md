# COMMAND LIST

<br>

## **IMPORTANT NOTES**

> THE COMMAND CAN BE CHANGED IN TIME BE SURE TO CHECK THIS OUT

> FOR OTHER COMMANDS WILL BE ADDED SOON

> NO EDITING FROM DATABASE PROHIBITED

> IF YOU WANT TO CHANGE SPECIFIC QUIZ OR SOMETHING JUST DROP IT WITH THE COMMAND AND ADD IT AGAIN WITH DIFFERENT DATA

<br>

## **STARTER PACK COMMAND**

* For participating in quiz

> -join

* For going out from quiz

> -out

> *NB : IF YOU GO OUT FROM QUIZ WHEN IT HAS BEEN STARTED YOU CANT JOIN BACK BEFORE THE QUIZ FINISHED 

* For Showing Profile

> -stats
    
<br>

## **QUIZ COMMAND**

* For Showing The Quiz [ONLY FOR QUIZ MANAGER] [ONLY WORK IF THERE IS NO QUIZ HAS BEEN STARTED]

> -quiz number(1-10^5)

* For Canceling The Quiz [ONLY FOR QUIZ MANAGER] [ONLY WORK IF QUIZ HAS BEEN STARTED OR BEFORE TIME RUN OUT] 

> -quiz cancel

* For Answering The Quiz [ONLY FOR PARTICIPANT] [ONLY WORK IF QUIZ HAS BEEN STARTED OR BEFORE TIME RUN OUT]

> -q text

<br>

## **USING ITEM COMMAND** [ONLY FOR PARTICIPANT]

* For Showing Invent 

> -inv show

* For Using item [Active and Targetable]

> -use item number[1-3] tag-player [Player Name that want to be targeted]

* For using item [Active and self-target only]

> -use item number[1-3]

* For using item [Passive]

> you cant use passive items

* For dropping item from inventory

> -inv drop number[1-3]

<br>

## **DATABASES COMMAND** [ONLY ADMIN AND QUIZ MANAGER]

* For Showing All Players

> -show players

* For Showing Player with name or number

> -show player name

> -show player number(1-10^5)

* For Showing Quizzes

> -show quizzes

* For Showing Quizzes with specific word or value

> -show quiz number(1-10^5)

> -show quiz type("image / sv / ost / others")

> -show quiz diff("ez / med / hard / ext / imm")

> -show quiz drop("item number")

> -show quiz + combination of all above

* For Showing Listed Items

> -show items

* For Showing Listed Items with specific word or value

> -show item number(1-10^5)

> -show item name("item name")

> -show item type("target / self / random / pass")

> -show item active(true/false)

> -show item value("like ammount of damage or heal")

> -show item rarity("comm / uncomm / etc")

> -show item + combination of all above

<br>

## **ADDING COMMAND** [ONLY ADMIN AND QUIZ MANAGER]

* For Adding Quizzes

> -add quiz type("image / sv / ost / bonus / voice-sv") file("embedded location like imgur / etc") diff("ez / med / hard / ext / imm") drop("item number") correctAnswer("the correct answer from your quiz") [Hint added in different command]

* For Adding Hints

> -add hint quiznumber("number of quiz that you wanted to insert hint") hints("split with space")[Hint must be 3]

* For Adding Items

> -add item name("item name) type("target / self / random / passive") active(true/false) value("like ammount of damage or heal") rarity("comm / uncomm / etc") description ("description of the item")

> *NB : FOR PASSIVE ITEM YOU MUST MAKE THE TYPE PASS

* For Adding Player via Admin

> -add player number(1-10^5) name discordID

<br>

## **DROPPING COMMAND** [ONLY ADMIN AND QUIZ MANAGER]

* For Dropping Player via Admin

> -drop player number(1-10^5)

> -drop player name

* For Dropping Item From Inventory Player via Admin

> -drop item number(1-10^5)[Player number] number(1-3)[Item number in inventory] 

> -drop item name[Player name] number(1-3)[Item number in inventory]

* For Dropping Item from Item Database

> -drop item number(1-10^5)

> -drop item name


* For Dropping Quiz From Quiz database

> -drop quiz number(1-10^5)

<br>

## **MEME COMMAND** [STILL ON WORK]

* ----------

