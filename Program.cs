using System;
using System.Collections.Generic;
using System.Numerics;

public enum Suit { Hearts, Diamonds, Clubs, Spades }
public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

// 카드 한 장을 표현하는 클래스
public class Card
{
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }

    public Card(Suit s, Rank r)
    {
        Suit = s;
        Rank = r;
    }

    public int GetValue()
    {
        if ((int)Rank <= 10)
        {
            return (int)Rank;
        }
        else if ((int)Rank <= 13)
        {
            return 10;
        }
        else
        {
            return 11;
        }
    }

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }
}

// 덱을 표현하는 클래스
public class Deck
{
    private List<Card> cards;

    public Deck()
    {
        cards = new List<Card>();

        foreach (Suit s in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank r in Enum.GetValues(typeof(Rank)))
            {
                cards.Add(new Card(s, r));
            }
        }

        Shuffle();
    }

    public void Shuffle()
    {
        Random rand = new Random();

        for (int i = 0; i < cards.Count; i++)
        {
            int j = rand.Next(i, cards.Count);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    public Card DrawCard()
    {
        Card card = cards[0];
        cards.RemoveAt(0);
        return card;
    }
}

// 패를 표현하는 클래스
public class Hand
{
    private List<Card> cards;

    public Hand()
    {
        cards = new List<Card>();
    }

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public int GetTotalValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (Card card in cards)
        {
            //Console.WriteLine(card.GetValue());
            total += card.GetValue();
            if (card.Rank == Rank.Ace)
            {
                aceCount++;
            }
        }

        while (total > 21 && aceCount > 0)
        {
            total -= 10;
            aceCount--;
        }

        return total;
    }



}

// 플레이어 클래스
public class Player
{
    public Hand Hand { get; private set; }

    public Player()
    {
        Hand = new Hand();
    }

    public Card DrawCardFromDeck(Deck deck)
    {
        Card drawnCard = deck.DrawCard();
        Hand.AddCard(drawnCard);
        return drawnCard;
    }


}

// 딜러 클래스, 부모 클래스인 플레이어로부터 상속받음
public class Dealer : Player
{

}

// 블랙잭 게임
public class Blackjack
{
    private Deck deck;
    private Player player;
    private Dealer dealer;
    private Card firstPlayerCard;
    private Card secondPlayerCard;
    private Card firstDealerCard;
    private Card secondDealerCard;

    public Blackjack()
    {
        deck = new Deck();
        player = new Player();
        dealer = new Dealer();
    }

    public void StartGame()
    {
        // 게임 시작 시 플레이어와 딜러에게 각각 두 장의 카드를 나눠줌
        firstPlayerCard = player.DrawCardFromDeck(deck);
        Console.WriteLine("플레이어의 첫 번째 카드 : " + firstPlayerCard);
        secondPlayerCard = player.DrawCardFromDeck(deck);
        Console.WriteLine("플레이어의 두 번째 카드 : " + secondPlayerCard);


        firstDealerCard = dealer.DrawCardFromDeck(deck);
        Console.WriteLine("딜러의 첫 번째 카드 : " + firstDealerCard);
        secondDealerCard = dealer.DrawCardFromDeck(deck);
        Console.WriteLine("딜러의 두 번째 카드 : 미공개");
        Console.WriteLine("\n");



        // 게임 메인 루프
        while (true)
        {
            PlayerTurn();
            if (player.Hand.GetTotalValue() > 21)
            {
                // 플레이어의 턴이 끝나면, 만약 플레이어의 카드 합계가 21을 초과하면 게임 종료
                break;
            }
            DealerTurn();
            if (dealer.Hand.GetTotalValue() > 17)
            {
                // 딜러의 턴이 끝나면, 만약 딜러의 카드 합계가 17을 초과하면 게임 종료
                break;
            }

            //int playerTotal = player.Hand.GetTotalValue();
            //int dealerTotal = dealer.Hand.GetTotalValue();

            //Console.WriteLine("\n");
            //Console.WriteLine($"플레이어의 최종 카드 합계: {playerTotal}");
            //Console.WriteLine($"딜러의 최종 카드 합계: {dealerTotal}");

            //break;




        }
        DisplayGameResult();
        //게임을 다시 시작할지 여부를 물어보고, 다시 시작하지 않으면 게임 종료
        if (PlayAgain())
        {
            // 게임을 다시 시작
            ResetGame(); // 덱을 다시 섞고 초기 상태로 돌아가도록 메서드를 추가
        }
    }

    private void ResetGame()
    {
        // 덱을 다시 초기화하고 플레이어와 딜러의 Hand 초기화, 게임 재시작
        deck = new Deck();
        player = new Player();
        dealer = new Dealer();

        // 게임을 시작하는 부분으로 이동
        StartGame();
    }

    private void PlayerTurn()
    {
        while (true)
        {
            int playerTotal = player.Hand.GetTotalValue();
            Console.WriteLine($"플레이어의 현재 카드 합: {playerTotal}");

            if (playerTotal <= 21)
            {
                Console.WriteLine("히트(1) 또는 스탠드(2)를 선택하세요: ");
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice == 1)
                    {
                        // 플레이어가 히트를 선택한 경우
                        player.DrawCardFromDeck(deck);
                    }
                    else if (choice == 2)
                    {
                        // 플레이어가 스탠드를 선택한 경우
                        break; // 플레이어의 턴 종료
                    }
                    else
                    {
                        Console.WriteLine("잘못된 선택입니다. 1 또는 2를 입력하세요.");
                    }
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다. 1 또는 2를 입력하세요.");
                }
            }
            else
            {
                // 플레이어 카드 합계가 21을 초과한 경우
                Console.WriteLine("플레이어의 카드 합계가 21을 초과했습니다!");
                break; // 플레이어의 턴 종료
            }
        }
        Console.WriteLine("\n");
    }


    private void DealerTurn()
    {
        while (true)
        {
            int dealerTotal = dealer.Hand.GetTotalValue();
            Console.WriteLine($"딜러의 현재 카드 합: {dealerTotal}");

            if (dealerTotal <= 17)
            {
                // 딜러가 카드 합계 17 미만이면 히트를 선택
                Console.WriteLine("\n딜러가 선택 중 . . . \n");
                dealer.DrawCardFromDeck(deck);
            }
            else
            {
                // 딜러가 카드 합계 17 이상이면 스탠드 선택
                break; // 딜러의 턴 종료
            }
        }
    }



    private void DisplayGameResult()
    {
        // 게임 결과를 표시하는 로직
        // 플레이어와 딜러의 최종 카드 합계를 비교하여 승패 결정 가능
        // 결과 및 게임의 승자를 콘솔에 출력

        int playerTotal = player.Hand.GetTotalValue();
        int dealerTotal = dealer.Hand.GetTotalValue();

        Console.WriteLine("\n");
        Console.WriteLine($"플레이어의 최종 카드 합계: {playerTotal}");
        Console.WriteLine($"딜러의 최종 카드 합계: {dealerTotal}\n");

        if (playerTotal > 21)
        {
            Console.WriteLine("플레이어가 버스트하여 딜러 승리!");
        }
        else if (dealerTotal > 21)
        {
            Console.WriteLine("딜러가 버스트하여 플레이어 승리!");
        }
        else if (playerTotal == dealerTotal)
        {
            Console.WriteLine("무승부!");
        }
        else if (playerTotal > dealerTotal)
        {
            Console.WriteLine("플레이어 승리!");
        }
        else
        {
            Console.WriteLine("딜러 승리!");
        }
    }


    private bool PlayAgain()
    {
        // 게임을 다시 시작할지 여부를 물어보고, 사용자의 입력에 따라 결정
        Console.WriteLine("게임을 재시작하시겠습니까? 재시작 : 1, 종료 : 2");
        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            if (choice == 1)
            {
                // 플레이어가 재시작을 선택한 경우
                return true;
            }
            else if (choice == 2)
            {
                // 플레이어가 종료를 선택한 경우
                return false;
            }
            else
            {
                Console.WriteLine("잘못된 선택입니다. 1 또는 2를 입력하세요.");
            }
        }
        else
        {
            Console.WriteLine("잘못된 입력입니다. 1 또는 2를 입력하세요.");
        }
        return false;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // 블랙잭 게임 실행
        Blackjack game = new Blackjack();
        game.StartGame();
    }
}