import type{ Dispatch } from 'redux';
import axios from 'axios';
import {ActionTypes} from './types';
import type {
  Character,
  ApiInfo,
  AppAction,
  AppState

} from './types';

interface ApiCharacter {
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
  image: string;
  episode: string[];
  url: string;
}

interface Filters {
  name?: string;
  status?: string;
  species?: string;
  type?: string;
  gender?: string;
}

const apiUrl = import.meta.env.VITE_REACT_APP_API_URL || '';

export const setCharacters = (characters: Character[]): AppAction => ({
  type: ActionTypes.SET_CHARACTERS,
  payload: characters,
});

export const toggleFavorite = (id: number): any => {
  return async (dispatch: Dispatch<AppAction>, getState: () => AppState) => {
    const state = getState();
    const char = state.characters.find(c => c.id === id);
    const isCurrentlyFavorite = char?.isFavorite;

    const token = sessionStorage.getItem('token');
    const email = sessionStorage.getItem('email');
    if (!token || !email) {
      console.error('No token or email found in sessionStorage');
      return;
    }
    try {
      if (isCurrentlyFavorite) {
        await axios.delete(`${apiUrl}/api/Favorites/${id}`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });
      } else {
        await axios.post(
          `${apiUrl}/api/Favorites`,
          { id, email },
          {
            headers: {
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json',
            },
          }
        );
      }
      dispatch({
        type: ActionTypes.TOGGLE_FAVORITE,
        payload: id,
      });
    } catch (error) {
      console.error('Error toggling favorite:', error);
    }
  };
};

export const setLoading = (loading: boolean): AppAction => ({
  type: ActionTypes.SET_LOADING,
  payload: loading,
});

export const setInfo = (info: ApiInfo): AppAction => ({
  type: ActionTypes.SET_INFO,
  payload: info,
});

export const setCurrentPage = (page: number): AppAction => ({
  type: ActionTypes.SET_CURRENT_PAGE,
  payload: page,
});

export const fetchCharacters = (
  page = 1,
  filters: Filters = {}
) => {
  return async (dispatch: Dispatch<AppAction>) => {
    dispatch(setLoading(true));
    try {
      const params = new URLSearchParams({ page: String(page) });
      Object.entries(filters).forEach(([key, value]) => {
        if (value) params.append(key, value);
      });
      const response = await axios.get(
        `https://rickandmortyapi.com/api/character/?${params.toString()}`
      );
      const favorites = await fetchFavorites();
      const favoriteIds = Array.isArray(favorites)
        ? favorites.map((fav: any) => fav.id)
        : [];
      const { results, info } = response.data;
      const characters: Character[] = results.map((char: ApiCharacter) => ({
        ...char,
        isFavorite: favoriteIds.includes(char.id),
      }));
      dispatch(setCharacters(characters));
      dispatch(setInfo(info));
      dispatch(setCurrentPage(page));
    } catch (error) {
      console.error('Error fetching characters:', error);
      dispatch(setCharacters([]));
      dispatch(setInfo({ count: 0, pages: 0, next: null, prev: null }));
    } finally {
      dispatch(setLoading(false));
    }
  };
};

export const fetchFavorites = async (): Promise<any[] | null> => {
  try {
    const token = sessionStorage.getItem('token');
    console.log('Token:', token);
    if (!token) {
      console.error('No token found in sessionStorage');
      return [];
    }
    const response = await axios.get(`${apiUrl}/api/Favorites`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    return response.data; 
  } catch (error) {
    console.error('Error fetching favorites:', error);
    return [];
  }
}