#pragma once
#include "common.hpp"

class url_parser
{
public:
	static std::string url_decode(const std::string& url) {
		std::string result;
		char ch;
		int step, char_byte;
		for (step = 0; step < url.size(); step++) {
			if (int(url[step]) == 37) {
				const int ret = sscanf(
					url.substr(
						static_cast<std::basic_string<char, std::char_traits<char>, std::allocator<char>>::size_type>(step) + 1, 
						2
					).c_str(),
					"%x",
					&char_byte
				);
				ch = static_cast<char>(char_byte);
				result += ch;
				step = step + 2;
			}
			else {
				result += url[step];
			}
		}
		return result;
	}

	static std::string url_encode(const std::string& url) {
		std::string result;
		for (auto& ch : url) {
			if (isalnum(ch) || ch == '-' || ch == '_' || ch == '.' || ch == '~') {
				result += ch;
			}
			else {
				result += '%' + std::to_string(static_cast<int>(ch));
			}
		}
		return result;
	}
};