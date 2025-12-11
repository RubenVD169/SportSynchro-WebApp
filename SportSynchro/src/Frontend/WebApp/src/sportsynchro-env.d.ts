interface Sport {
  id: number;
  name: string;
  visible: boolean;
}

interface League {
  id: number;
  sportId: number;
  name: string;
  visible: boolean;
}
