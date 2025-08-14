export interface ApiInfo {
  count: number;
  pages: number;
  next: string | null;
  prev: string | null;
}

export interface Character {
  id: number;
  name: string;
  status: string;
  species: string;
  type: string;
  gender: string;
  origin: {
    name: string;
    url: string;
  };
  location: {
    name: string;
    url: string;
  };
  image: string;
  episode: string[]; 
  isFavorite?: boolean;
}


export interface AppState {
  characters: Character[];
  info: ApiInfo | null;
  loading: boolean;
  currentPage: number;
  pageSize: number; 
  totalItems: number; 
}

export const ActionTypes = {
  SET_CHARACTERS: 'SET_CHARACTERS',
  TOGGLE_FAVORITE: 'TOGGLE_FAVORITE',
  SET_LOADING: 'SET_LOADING',
  SET_INFO: 'SET_INFO',
  SET_CURRENT_PAGE: 'SET_CURRENT_PAGE',
  SET_PAGE_SIZE: 'SET_PAGE_SIZE',
  SET_TOTAL_ITEMS: 'SET_TOTAL_ITEMS',
} as const;

export type ActionType = typeof ActionTypes[keyof typeof ActionTypes];

interface SetCharactersAction {
  type: typeof ActionTypes.SET_CHARACTERS;
  payload: Character[];
}

interface ToggleFavoriteAction {
  type: typeof ActionTypes.TOGGLE_FAVORITE;
  payload: number;
}

interface SetLoadingAction {
  type: typeof ActionTypes.SET_LOADING;
  payload: boolean;
}

interface SetInfoAction {
  type: typeof ActionTypes.SET_INFO;
  payload: ApiInfo;
}

interface SetCurrentPageAction {
  type: typeof ActionTypes.SET_CURRENT_PAGE;
  payload: number;
}

interface SetPageSizeAction {
  type: typeof ActionTypes.SET_PAGE_SIZE;
  payload: number;
}

interface SetTotalItemsAction {
  type: typeof ActionTypes.SET_TOTAL_ITEMS;
  payload: number;
}

export type AppAction =
  | SetCharactersAction
  | ToggleFavoriteAction
  | SetLoadingAction
  | SetInfoAction
  | SetCurrentPageAction
  | SetPageSizeAction
  | SetTotalItemsAction;
