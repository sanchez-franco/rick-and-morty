const validStatus = ['alive', 'dead', 'unknown'] as const;
const validGender = ['female', 'male', 'genderless', 'unknown'] as const;

export interface Filters {
  name?: string;
  status?: string;
  gender?: string;
}

export function parseFilters(input: string): Filters {
  const words = input.toLowerCase().split(' ').filter(Boolean);

  let status: string | undefined;
  let gender: string | undefined;
  let nameParts: string[] = [];

  for (const word of words) {
    if (validStatus.includes(word as typeof validStatus[number])) {
      status = word;
    } else if (validGender.includes(word as typeof validGender[number])) {
      gender = word;
    } else {
      nameParts.push(word);
    }
  }

  const name = nameParts.length ? nameParts.join(' ') : undefined;

  return { name, status, gender };
}
